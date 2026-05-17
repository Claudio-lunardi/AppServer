using AppServer.Shared.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AppServer.Infrastructure.RabbitMq;

public class RabbitMQFactory : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly string _clientProvidedName;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public RabbitMQFactory(IOptions<RabbitMqConfig> options, RabbitMqConnectionIdentity connectionIdentity)
    {
        var config = options.Value;
        _factory = new ConnectionFactory
        {
            HostName = config.HostName,
            Port = config.Port,
            UserName = config.UserName,
            Password = config.Password,
        };
        _clientProvidedName = connectionIdentity.ClientProvidedName;
    }

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            _connection = await _factory.CreateConnectionAsync(_clientProvidedName, cancellationToken);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();

        _connectionLock.Dispose();
    }
}
