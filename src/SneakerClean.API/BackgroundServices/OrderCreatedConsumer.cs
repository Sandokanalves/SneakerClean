using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SneakerClean.Domain.Events;

namespace SneakerClean.API.BackgroundServices;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public OrderCreatedConsumer(IConfiguration configuration, ILogger<OrderCreatedConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Política de retry sênior: tenta reconectar com Exponential Backoff
        _retryPolicy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s, 16s, 32s
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "[RabbitMQ Consumer] Falha ao conectar (Tentativa {RetryCount}/5). Aguardando {Seconds}s...", retryCount, timespan.TotalSeconds);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hostName = _configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var factory = new ConnectionFactory { HostName = hostName };

        // O Polly envolve a criação da conexão de forma limpa e declarativa
        var connection = await _retryPolicy.ExecuteAsync(async () =>
            await factory.CreateConnectionAsync(stoppingToken));

        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "orders_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

            _logger.LogInformation("[RabbitMQ Consumer] Pedido Recebido ID: {OrderId} - Cliente: {CustomerName}", orderEvent?.OrderId, orderEvent?.CustomerName);
            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: "orders_queue", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
    }
}