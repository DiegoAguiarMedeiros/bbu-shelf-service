namespace Application.Interfaces;

public interface IKeycloakService
{
    /// <summary>
    /// Fetches a user's profile from Keycloak Admin API.
    /// Returns null (silently) when Keycloak is unavailable or the user is not found.
    /// </summary>
    Task<KeycloakUserInfo?> GetUserAsync(Guid keycloakId, CancellationToken cancellationToken = default);
}

public record KeycloakUserInfo(string? id, string? Username, string? FirstName, string? LastName, string? Email, long? CreatedTimestamp)
{
    /// <summary>Full display name combining first and last name.</summary>
    public string? FullName =>
        string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? Username
            : $"{FirstName} {LastName}".Trim();

    /// <summary>Date and time the account was created, converted from the Keycloak Unix timestamp (ms).</summary>
    public DateTimeOffset? CreatedAt =>
        CreatedTimestamp.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(CreatedTimestamp.Value)
            : null;
}
