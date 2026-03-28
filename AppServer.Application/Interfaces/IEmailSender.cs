namespace AppServer.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
}
