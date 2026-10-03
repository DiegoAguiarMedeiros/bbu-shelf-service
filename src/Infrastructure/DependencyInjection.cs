using Application.Interfaces;
using Infrastructure.Keycloak;
using Infrastructure.Messaging.Events;
using Infrastructure.Messaging.RabbitMQ;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.IShelfRepository;
using Application.Interfaces.ITradeOfferRepository;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Database ─────────────────────────────────────────────────────────
        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");

        if (useInMemory)
        {
            services.AddDbContext<ShelfDbContext>(options =>
                options.UseInMemoryDatabase("ShelfDb"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Database=ShelfDb;Username=postgres;Password=postgres";

            services.AddDbContext<ShelfDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });
        }

        services.AddScoped<IShelfRepository, ShelfRepository>();
        services.AddScoped<ITradeOfferRepository, TradeOfferRepository>();

        // ── Messaging ────────────────────────────────────────────────────────
        services.AddSingleton<RabbitMqPublisher>();
        services.AddHostedService<ShelfCreatedConsumer>();

        // ── Keycloak ─────────────────────────────────────────────────────────
        services.AddHttpContextAccessor();

        services.Configure<KeycloakOptions>(
            configuration.GetSection(KeycloakOptions.SectionName));

        services.AddHttpClient<IKeycloakService, KeycloakService>();

        return services;
    }
}
