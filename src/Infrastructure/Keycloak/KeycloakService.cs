using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Keycloak;

/// <summary>
/// Calls Keycloak or decodes JWT claims to retrieve user profile data (name + email).
/// Strategy 1: Decodes claims directly from the incoming HTTP request's Bearer JWT token (instant, 0ms, no network calls).
/// Strategy 2: Calls Keycloak /userinfo endpoint using the user's Bearer token.
/// Strategy 3: Fallback to Client Credentials flow via Keycloak Admin API.
/// </summary>
public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _http;
    private readonly KeycloakOptions _opts;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<KeycloakService> _logger;

    private string? _cachedToken;
    private DateTime _tokenExpiresAt = DateTime.MinValue;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    public KeycloakService(
        HttpClient http,
        IOptions<KeycloakOptions> opts,
        IHttpContextAccessor httpContextAccessor,
        ILogger<KeycloakService> logger)
    {
        _http = http;
        _opts = opts.Value;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<KeycloakUserInfo?> GetUserAsync(Guid keycloakId, CancellationToken cancellationToken = default)
    {
        var jwtClaimsInfo = TryGetUserInfoFromJwtToken();
        var userContextInfo = jwtClaimsInfo ?? await TryGetUserInfoFromUserTokenAsync(cancellationToken);

        // Sempre enriquece com dados da Admin API se precisar de CreatedTimestamp
        var adminInfo = await TryGetUserFromAdminApiAsync(keycloakId, cancellationToken);

        if (userContextInfo is not null && adminInfo is not null)
        {
            return userContextInfo with { id = adminInfo.id, CreatedTimestamp = adminInfo.CreatedTimestamp };
        }

        return adminInfo ?? userContextInfo;
    }

    // ── Strategy 1: Instant Local JWT Claims Decoding ───────────────────────

    private KeycloakUserInfo? TryGetUserInfoFromJwtToken()
    {
        try
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var userToken = authHeader["Bearer ".Length..].Trim();
            if (string.IsNullOrEmpty(userToken)) return null;

            var parts = userToken.Split('.');
            if (parts.Length < 2) return null;

            var payloadJson = Base64UrlDecode(parts[1]);
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;

            var id = root.TryGetProperty("id", out var i) ? i.GetString() : null;
            var username = root.TryGetProperty("preferred_username", out var u) ? u.GetString() : null;
            var firstName = root.TryGetProperty("given_name", out var g) ? g.GetString() : null;
            var lastName = root.TryGetProperty("family_name", out var f) ? f.GetString() : null;
            var email = root.TryGetProperty("email", out var e) ? e.GetString() : null;
            var name = root.TryGetProperty("name", out var n) ? n.GetString() : null;

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(name))
            {
                var nameParts = name.Split(' ', 2);
                firstName = nameParts[0];
                lastName = nameParts.Length > 1 ? nameParts[1] : null;
            }

            if (username is null && firstName is null && email is null) return null;

            return new KeycloakUserInfo(
                id: id,
                Username: username ?? firstName,
                FirstName: firstName,
                LastName: lastName,
                Email: email,
                CreatedTimestamp: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to parse userinfo from JWT token payload");
            return null;
        }
    }

    private static string Base64UrlDecode(string input)
    {
        var output = input.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 2: output += "=="; break;
            case 3: output += "="; break;
        }
        var converted = Convert.FromBase64String(output);
        return System.Text.Encoding.UTF8.GetString(converted);
    }

    // ── Strategy 2: User Bearer Token /userinfo ──────────────────────────────

    private async Task<KeycloakUserInfo?> TryGetUserInfoFromUserTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var userToken = authHeader["Bearer ".Length..].Trim();
            if (string.IsNullOrEmpty(userToken)) return null;

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_opts.BaseUrl}/realms/{_opts.Realm}/protocol/openid-connect/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var userInfo = await response.Content.ReadFromJsonAsync<KeycloakUserInfoRepresentation>(cancellationToken: cancellationToken);
            if (userInfo is null) return null;

            return new KeycloakUserInfo(
                id: userInfo.id,
                Username: userInfo.PreferredUsername ?? userInfo.GivenName,
                FirstName: userInfo.GivenName,
                LastName: userInfo.FamilyName,
                Email: userInfo.Email,
                CreatedTimestamp: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to fetch userinfo using incoming Bearer token");
            return null;
        }
    }

    private async Task<KeycloakUserInfo?> TryGetUserFromAdminApiAsync(Guid keycloakId, CancellationToken cancellationToken)
    {
        try
        {
            var adminToken = await GetAdminTokenAsync(cancellationToken);
            if (adminToken is null)
            {
                _logger.LogWarning("Could not obtain a service-account token to query Keycloak users");
                return null;
            }

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_opts.BaseUrl}/admin/realms/{_opts.Realm}/users/{keycloakId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Keycloak Admin API returned {StatusCode} while fetching user {Id}",
                    (int)response.StatusCode,
                    keycloakId);
                return null;
            }

            var body = await response.Content.ReadFromJsonAsync<KeycloakUserRepresentation>(cancellationToken: cancellationToken);
            return body is null
                ? null
                : new KeycloakUserInfo(
                    id: body.id,
                    Username: body.Username,
                    FirstName: body.FirstName,
                    LastName: body.LastName,
                    Email: body.Email,
                    CreatedTimestamp: body.CreatedTimestamp);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not fetch user {Id} from Keycloak Admin API", keycloakId);
            return null;
        }
    }
    // ── Strategy 3: Client Credentials Service Account ──────────────────────

    private async Task<string?> GetAdminTokenAsync(CancellationToken cancellationToken)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiresAt)
            return _cachedToken;

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiresAt)
                return _cachedToken;

            if (string.IsNullOrWhiteSpace(_opts.ClientSecret) || _opts.ClientSecret == "CHANGE_ME")
            {
                _logger.LogWarning("Keycloak client secret is not configured");
                return null;
            }

            var response = await _http.PostAsync(
                $"{_opts.BaseUrl}/realms/{_opts.Realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _opts.ClientId,
                    ["client_secret"] = _opts.ClientSecret,
                }),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Keycloak token endpoint returned {StatusCode} for client {ClientId}",
                    (int)response.StatusCode,
                    _opts.ClientId);
                return null;
            }

            var tokenResponse = await response.Content
                .ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken: cancellationToken);

            if (tokenResponse is null) return null;

            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30);

            return _cachedToken;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not obtain a Keycloak service-account token");
            return null;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    // ── Deserialization Models ────────────────────────────────────────────────

    private sealed record KeycloakUserInfoRepresentation(
        [property: JsonPropertyName("sub")] string? id,
        [property: JsonPropertyName("preferred_username")] string? PreferredUsername,
        [property: JsonPropertyName("given_name")] string? GivenName,
        [property: JsonPropertyName("family_name")] string? FamilyName,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("email")] string? Email
    );

    private sealed record KeycloakTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );

    private sealed record KeycloakUserRepresentation(
        [property: JsonPropertyName("id")] string? id,
        [property: JsonPropertyName("username")] string? Username,
        [property: JsonPropertyName("firstName")] string? FirstName,
        [property: JsonPropertyName("lastName")] string? LastName,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("createdTimestamp")] long? CreatedTimestamp
    );
}
