using AppServer.Application.Email.QueueEmail;
using AppServer.Shared.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

using AppServer.Infrastructure.RabbitMq;

namespace AppServer.Infrastructure.Email.Queue.Publisher;

public class RabbitMqEmailQueuePublisher : IEmailQueuePublisher
{
    private readonly ILogger<RabbitMqEmailQueuePublisher> _logger;
    private readonly RabbitMQFactory _factory;

    public RabbitMqEmailQueuePublisher(ILogger<RabbitMqEmailQueuePublisher> logger, RabbitMQFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task PublishAsync(EmailMessageModel message, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var channel = await _factory.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "email-queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "email-queue",
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Mensagem publicada na fila para {To}", message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem na fila");
            throw;
        }
    }
}
