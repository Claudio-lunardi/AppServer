using AppServer.Infrastructure;
using AppServer.Shared.Config;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("Email"));

builder.Services.AddInfrastructure();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AppServer API",
        Description = "An ASP.NET Core Web API for managing AppServer.",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AppServer API v1");
        options.RoutePrefix = "swagger";
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
