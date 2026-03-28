using AppServer.Shared.Models;

namespace AppServer.Application.Interfaces;

public interface IEmailPublisher
{
    Task PublishAsync(EmailMessageModel message);
}
