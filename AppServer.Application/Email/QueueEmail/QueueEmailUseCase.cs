using AppServer.Shared.Models;
namespace AppServer.Application.Email.QueueEmail;

public class QueueEmailUseCase : IQueueEmailUseCase
{
    private readonly IEmailQueuePublisher _publisher;

    public QueueEmailUseCase(IEmailQueuePublisher publisher)
    {
        _publisher = publisher;
    }

    public Task QueueAsync(EmailMessageModel message, CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(message, cancellationToken);
    }
}
