using AppServer.Shared.Models;

namespace AppServer.Application.Email.ProcessQueuedEmail;

public interface IProcessQueuedEmailUseCase
{
    Task ProcessAsync(EmailMessageModel message, CancellationToken cancellationToken = default);
}
