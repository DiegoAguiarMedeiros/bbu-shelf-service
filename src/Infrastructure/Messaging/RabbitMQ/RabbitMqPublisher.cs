using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.RabbitMQ;

public class RabbitMqPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default)
    {
        var hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";
        var factory = new ConnectionFactory { HostName = hostName };

        try
        {
            using var connection = await factory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Published event to RabbitMQ queue '{QueueName}': {Message}", queueName, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ queue '{QueueName}'", queueName);
        }
    }
}
