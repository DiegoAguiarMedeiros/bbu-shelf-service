using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;

namespace Api.Extensions;

public static class DependencyInjection
{
    public const string CorsPolicyName = "AllowFrontend";

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Shelf API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole apenas o token JWT (sem o prefixo 'Bearer')."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });
        });

        var keycloakBaseUrl = configuration["Keycloak:BaseUrl"]?.TrimEnd('/');
        var keycloakPublicUrl = configuration["Keycloak:PublicUrl"]?.TrimEnd('/') ?? keycloakBaseUrl;
        var keycloakRealm = configuration["Keycloak:Realm"];
        var keycloakClientId = configuration["Keycloak:ClientId"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false; // Evita que o ASP.NET converta "sub" para ClaimTypes.NameIdentifier
                options.Authority = $"{keycloakBaseUrl}/realms/{keycloakRealm}";
                options.Audience = keycloakClientId;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = $"{keycloakPublicUrl}/realms/{keycloakRealm}",
                    ValidateIssuer = true,
                    ValidateAudience = false,
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                        logger.LogError(context.Exception, "JWT Authentication failed.");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                        logger.LogInformation("JWT Authentication succeeded for subject: {Subject}",
                            context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                        return Task.CompletedTask;
                    }
                };
                options.RequireHttpsMetadata = false;
            });
        services.AddAuthorization();

        // ── CORS ─────────────────────────────────────────────────────────────
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                if (allowedOrigins is { Length: > 0 })
                    policy.WithOrigins(allowedOrigins);
                else
                    policy.WithOrigins(
                        "http://localhost:3000",  // Docker frontend
                        "http://localhost:5173"   // Vite dev server
                    );

                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
