using AppServer.Shared.Models;

namespace AppServer.Application.Interfaces;

public interface IRabbitMqConsumer
{
    Task StartAsync(Func<EmailMessageModel, Task> handler, CancellationToken cancellationToken);
}
