using Whisprr.BlueskyService;
using Whisprr.SocialScouter.Modules.Caching;
using Whisprr.SocialScouter.Modules.SocialListener;
// using Whisprr.Infrastructure.MessageBroker;
// using Whisprr.Infrastructure.Redis;
// using Whisprr.SocialScouter.Modules.SocialListener;

var builder = Host.CreateApplicationBuilder(args);

builder
    .AddCaching()
    .AddBlueskyServices()
    // .AddMessageBroker()
    .AddSocialListenerServices();

var host = builder.Build();
host.Run();
