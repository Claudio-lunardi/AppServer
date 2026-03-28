using AppServer.Application.Interfaces;
using AppServer.Shared.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AppServer.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailConfig _config;

    public EmailSender(ILogger<EmailSender> logger, IOptions<EmailConfig> options)
    {
        _logger = logger;
        _config = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.Auto, cancellationToken);
            await smtp.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("E-mail enviado para {To} | Assunto: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail para {To}", to);
            throw;
        }
    }
}
