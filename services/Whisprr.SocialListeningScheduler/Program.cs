using Whisprr.MessageBroker.Modules.Infrastructure;
using Whisprr.SocialListeningScheduler.Modules.HangfireWorker;

var builder = WebApplication.CreateBuilder(args);


builder.AddMessageBroker();
builder.Services.AddHangfireWorker();

var app = builder.Build();

app.UseHangfireWorker();

app.Run();
