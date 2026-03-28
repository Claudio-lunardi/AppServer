using AppServer.Application.Interfaces;
using AppServer.Infrastructure.Email;
using AppServer.Infrastructure.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace AppServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMQFactory>();
        services.AddScoped<IEmailPublisher, RabbitMqPublisher>();
        services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}
