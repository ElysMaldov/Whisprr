using Whisprr.MessageBroker.Modules.Infrastructure;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Modules.HangfireWorker;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddMessageBroker()
    .AddAppDbContext()
    .AddHangfireWorker();

var app = builder.Build();

app.UseHangfireWorker();

app.Run();
