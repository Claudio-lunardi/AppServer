namespace AppServer.Infrastructure.RabbitMq;

public sealed class RabbitMqConnectionIdentity
{
    public RabbitMqConnectionIdentity(string clientProvidedName)
    {
        ClientProvidedName = clientProvidedName;
    }

    public string ClientProvidedName { get; }
}
