using AppServer.Application.Email.SendEmail;
using AppServer.Shared.Models;

namespace AppServer.Application.Email.ProcessQueuedEmail;

public class ProcessQueuedEmailUseCase : IProcessQueuedEmailUseCase
{
    private readonly IEmailSender _emailSender;

    public ProcessQueuedEmailUseCase(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task ProcessAsync(EmailMessageModel message, CancellationToken cancellationToken = default)
    {
        return _emailSender.SendAsync(
            to: message.To,
            subject: message.Subject,
            body: message.Body,
            isHtml: message.IsHtml,
            cancellationToken: cancellationToken);
    }
}
