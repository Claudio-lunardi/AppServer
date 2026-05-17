using AppServer.Application.Email.ProcessQueuedEmail;
using AppServer.Shared.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

using AppServer.Infrastructure.RabbitMq;

namespace AppServer.Infrastructure.Email.Queue.Consumer;

public class RabbitMqEmailQueueConsumer : IEmailQueueConsumer
{
    private readonly ILogger<RabbitMqEmailQueueConsumer> _logger;
    private readonly RabbitMQFactory _factory;

    public RabbitMqEmailQueueConsumer(ILogger<RabbitMqEmailQueueConsumer> logger, RabbitMQFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task StartAsync(Func<EmailMessageModel, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var channel = await _factory.CreateChannelAsync(cancellationToken);

                await channel.QueueDeclareAsync(
                    queue: "email-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (_, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var message = JsonSerializer.Deserialize<EmailMessageModel>(json);

                        if (message != null)
                        {
                            await handler(message, cancellationToken);
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
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao conectar/consumir RabbitMQ. Nova tentativa em 5 segundos.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
