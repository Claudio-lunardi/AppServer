using AppServer.Shared.Models;

namespace AppServer.Application.Email.ProcessQueuedEmail;

public interface IEmailQueueConsumer
{
    Task StartAsync(Func<EmailMessageModel, CancellationToken, Task> handler, CancellationToken cancellationToken);
}
