using AppServer.Application.Email.ProcessQueuedEmail;
using AppServer.Application.Email.QueueEmail;
using Microsoft.Extensions.DependencyInjection;

namespace AppServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQueueEmailUseCase, QueueEmailUseCase>();
        services.AddScoped<IProcessQueuedEmailUseCase, ProcessQueuedEmailUseCase>();

        return services;
    }
}
