using AppServer.Shared.Models;

namespace AppServer.Application.Email.QueueEmail;

public interface IEmailQueuePublisher
{
    Task PublishAsync(EmailMessageModel message, CancellationToken cancellationToken = default);
}
