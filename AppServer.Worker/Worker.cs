using AppServer.Application.Interfaces;
using AppServer.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AppServer.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConsumer _rabbitMqConsumer;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IRabbitMqConsumer rabbitMqConsumer)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _rabbitMqConsumer = rabbitMqConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Inicia consumo da fila
            await _rabbitMqConsumer.StartAsync(handler: ProcessarMensagemAsync, cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no EmailWorker");
        }
    }

    private async Task ProcessarMensagemAsync(EmailMessageModel message)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        await emailSender.SendAsync(
            to: message.To,
            subject: message.Subject,
            body: message.Body,
            isHtml: message.IsHtml
        );
    }
}
