using MassTransit;
using Whisprr.MessageBroker.Modules.Infrastructure;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Modules.Hangfire;
using Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddMessageBroker(busCfg =>
    {
      // Add Entity Framework Core Transactional Outbox
      // This ensures database updates and message dispatches are atomic
      busCfg.AddEntityFrameworkOutbox<AppDbContext>(o =>
      {
        // Use PostgreSQL for the outbox tables
        o.UsePostgres();

        // Enable the bus outbox for automatic background delivery
        o.UseBusOutbox();
      });
    })
    .AddAppDbContext()
    .AddSocialListeningTaskPublisher()
    .AddHangfire();

var app = builder.Build();

app.UseHangfire();

// Run database seeder on startup if configured
await app.SeedDatabaseAsync();

app.Run();
