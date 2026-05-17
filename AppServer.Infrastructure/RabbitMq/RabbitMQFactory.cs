using AppServer.Shared.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AppServer.Infrastructure.RabbitMq;

public class RabbitMQFactory : IAsyncDisposable
{
    private readonly IConnection _connection;

    public RabbitMQFactory(IOptions<RabbitMqConfig> options, RabbitMqConnectionIdentity connectionIdentity)
    {
        var config = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = config.HostName,
            Port = config.Port,
            UserName = config.UserName,
            Password = config.Password,
        };

        _connection = factory
            .CreateConnectionAsync(connectionIdentity.ClientProvidedName, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    public Task<IChannel> CreateChannelAsync() => _connection.CreateChannelAsync();

    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();
}
