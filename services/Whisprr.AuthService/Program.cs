using Whisprr.AuthService.Infrastructure;
using Whisprr.MessageBroker.Modules.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAuthDbContext()
    .AddMessageBroker()
    .AddAuthServices()
    .AddApiServices()
    .AddApiDocumentation();

var app = builder.Build();

app
    .UseApiDocumentation()
    .UseAuthServices()
    .UseApiServices();

app.Run();
