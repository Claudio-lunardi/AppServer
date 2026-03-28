using AppServer.Application.Interfaces;
using AppServer.Infrastructure.RabbitMq;
using AppServer.Shared.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AppServer.Infrastructure.RabbitMq;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMQFactory _factory;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, RabbitMQFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task StartAsync(Func<EmailMessageModel, Task> handler, CancellationToken cancellationToken)
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

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<EmailMessageModel>(json);

                    if (message != null)
                    {
                        await handler(message);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem");
                    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: "email-queue",
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer
            );

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no consumidor RabbitMQ");
            throw;
        }
    }
}
