using AppServer.Application.Email.ProcessQueuedEmail;
using AppServer.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AppServer.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEmailQueueConsumer _emailQueueConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _delayMs;

    public Worker(
        ILogger<Worker> logger,
        IEmailQueueConsumer emailQueueConsumer,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _emailQueueConsumer = emailQueueConsumer;
        _serviceScopeFactory = serviceScopeFactory;
        _delayMs = ReadDelayMs();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _emailQueueConsumer.StartAsync(ProcessarMensagemAsync, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no EmailWorker");
        }
    }

    private Task ProcessarMensagemAsync(EmailMessageModel message, CancellationToken cancellationToken)
    {
        return ProcessarMensagemEmEscopoAsync(message, cancellationToken);
    }

    private async Task ProcessarMensagemEmEscopoAsync(EmailMessageModel message, CancellationToken cancellationToken)
    {
        if (_delayMs > 0)
        {
            _logger.LogInformation("Aguardando {DelayMs}ms antes de processar mensagem para {To}", _delayMs, message.To);
            await Task.Delay(_delayMs, cancellationToken);
        }

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var processQueuedEmailUseCase = scope.ServiceProvider.GetRequiredService<IProcessQueuedEmailUseCase>();

        await processQueuedEmailUseCase.ProcessAsync(message, cancellationToken);
    }

    private static int ReadDelayMs()
    {
        var rawValue = Environment.GetEnvironmentVariable("WORKER_DELAY_MS");

        return int.TryParse(rawValue, out var delayMs) && delayMs > 0
            ? delayMs
            : 0;
    }
}
