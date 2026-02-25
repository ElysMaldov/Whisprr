using Whisprr.AuthService.Infrastructure;
using Whisprr.MessageBroker.Modules.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder
    .AddAuthDbContext()
    .AddMessageBroker()
    .AddAuthServices()
    .AddApiDocumentation();

var app = builder.Build();

app
    .UseApiDocumentation()
    .UseAuthServices();

app.MapControllers();

app.Run();
