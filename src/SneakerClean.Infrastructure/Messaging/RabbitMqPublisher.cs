using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SneakerClean.Infrastructure.Messaging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using SneakerClean.Domain.Events;

namespace SneakerClean.Infrastructure.Messaging;

public class RabbitMqPublisher
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception, "[RabbitMQ Publisher] Falha ao publicar evento. Tentativa {RetryCount}/3 após {Seconds}s.", retryCount, timeSpan.TotalSeconds);
                });
    }

    public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "orders_queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(orderEvent);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "orders_queue",
                body: body);

            _logger.LogInformation("[RabbitMQ Publisher] Evento do Pedido {OrderId} publicado com sucesso.", orderEvent.OrderId);
        });
    }
}