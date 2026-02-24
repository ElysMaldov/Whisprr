using Whisprr.MessageBroker.Modules.Infrastructure;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Modules.Hangfire;
using Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddMessageBroker()
    .AddAppDbContext()
    .AddSocialListeningTaskPublisher()
    .AddHangfire();

var app = builder.Build();

app.UseHangfire();

// Run database seeder on startup if configured
await app.SeedDatabaseAsync();

app.Run();
