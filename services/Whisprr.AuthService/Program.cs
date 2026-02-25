using Whisprr.AuthService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddAuthDbContext()
    .AddAuthServices()
    .AddApiDocumentation();

var app = builder.Build();

app
    .UseApiDocumentation()
    .UseAuthServices();

await app.InitializeDatabaseAsync();

app.Run();
