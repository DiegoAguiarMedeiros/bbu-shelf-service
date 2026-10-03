using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Events;

public class ShelfCreatedConsumer : BackgroundService
{
    private readonly ILogger<ShelfCreatedConsumer> _logger;

    public ShelfCreatedConsumer(ILogger<ShelfCreatedConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ShelfCreatedConsumer background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
