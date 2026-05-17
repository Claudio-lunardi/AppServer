using AppServer.Application.Email.ProcessQueuedEmail;
using AppServer.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AppServer.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEmailQueueConsumer _emailQueueConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(
        ILogger<Worker> logger,
        IEmailQueueConsumer emailQueueConsumer,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _emailQueueConsumer = emailQueueConsumer;
        _serviceScopeFactory = serviceScopeFactory;
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
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var processQueuedEmailUseCase = scope.ServiceProvider.GetRequiredService<IProcessQueuedEmailUseCase>();

        await processQueuedEmailUseCase.ProcessAsync(message, cancellationToken);
    }
}
