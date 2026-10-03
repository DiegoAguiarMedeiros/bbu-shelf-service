using Api.Extensions;
using Api.Middlewares;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Program");
    var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");

    if (!useInMemory)
    {
        var maxAttempts = 8;
        var delayMs = 2000;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                logger.LogInformation("Ensuring database schema (attempt {Attempt}/{MaxAttempts})...", attempt, maxAttempts);
                var db = services.GetRequiredService<ShelfDbContext>();
                db.Database.EnsureCreated();
                logger.LogInformation("Database schema created/verified successfully.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Attempt {Attempt} to apply migrations failed.", attempt);
                if (attempt == maxAttempts)
                {
                    logger.LogError(ex, "Could not apply database migrations after {MaxAttempts} attempts.", maxAttempts);
                    throw;
                }

                Thread.Sleep(delayMs);
                delayMs = Math.Min(delayMs * 2, 30000);
            }
        }
    }
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(DependencyInjection.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
