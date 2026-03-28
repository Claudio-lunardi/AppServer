using AppServer.Application.Interfaces;
using AppServer.Shared.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AppServer.Infrastructure.RabbitMq;

public class RabbitMqPublisher : IEmailPublisher
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly RabbitMQFactory _factory;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger, RabbitMQFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task PublishAsync(EmailMessageModel message)
    {
        try
        {
            var channel = await _factory.GetChannelAsync();

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
