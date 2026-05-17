using AppServer.Application.Email.ProcessQueuedEmail;
using AppServer.Application.Email.QueueEmail;
using AppServer.Application.Email.SendEmail;
using AppServer.Infrastructure.Email.Queue.Consumer;
using AppServer.Infrastructure.Email.Queue.Publisher;
using AppServer.Infrastructure.Email.Sending;
using AppServer.Infrastructure.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace AppServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string rabbitMqClientProvidedName)
    {
        services.AddSingleton(new RabbitMqConnectionIdentity(rabbitMqClientProvidedName));
        services.AddSingleton<RabbitMQFactory>();
        services.AddSingleton<IEmailQueueConsumer, RabbitMqEmailQueueConsumer>();

        services.AddScoped<IEmailQueuePublisher, RabbitMqEmailQueuePublisher>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
