using Whisprr.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAppDbContext()
    .AddAuthenticationServices()
    .AddApiServices()
    .AddApiDocumentation();

var app = builder.Build();

app
    .UseApiDocumentation()
    .UseAuthenticationServices()
    .UseApiServices();

app.Run();
