using Whisprr.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAppDbContext()
    .AddApiServices()
    .AddApiDocumentation();

var app = builder.Build();

app
    .UseApiDocumentation()
    .UseApiServices();

await app.InitializeDatabaseAsync();

app.Run();
