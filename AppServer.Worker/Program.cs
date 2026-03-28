using AppServer.Infrastructure;
using AppServer.Shared.Config;
using AppServer.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("Email"));

builder.Services.AddInfrastructure();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
