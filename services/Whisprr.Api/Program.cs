using Whisprr.Api.Infrastructure;
using Whisprr.Api.MessageBroker.Consumers;
using Whisprr.MessageBroker.Modules.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAppDbContext()
    .AddMessageBroker(busCfg =>
    {
        busCfg.AddConsumer<SocialInfoCreatedConsumer>();
    })
    .AddAuthenticationServices()
    .AddApiServices()
    .AddApiDocumentation();

var app = builder.Build();

app
    .UseApiDocumentation()
    .UseAuthenticationServices()
    .UseApiServices();

app.Run();
