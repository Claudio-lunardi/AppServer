using AppServer.Shared.Models;

namespace AppServer.Application.Email.QueueEmail;

public interface IQueueEmailUseCase
{
    Task QueueAsync(EmailMessageModel message, CancellationToken cancellationToken = default);
}
