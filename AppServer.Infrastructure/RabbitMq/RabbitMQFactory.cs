using AppServer.Shared.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AppServer.Infrastructure.RabbitMq;

public class RabbitMQFactory : IAsyncDisposable
{
    private IChannel? _channel;
    private readonly IConnection _connection;

    public RabbitMQFactory(IOptions<RabbitMqConfig> options)
    {
        var config = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = config.HostName,
            Port = config.Port,
            UserName = config.UserName,
            Password = config.Password,
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public async Task<IChannel> GetChannelAsync()
    {
        if (_channel == null || !_channel.IsOpen)
        {
            if (_channel != null)
                await _channel.DisposeAsync();

            _channel = await _connection.CreateChannelAsync();
        }

        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();

        await _connection.DisposeAsync();
    }
}