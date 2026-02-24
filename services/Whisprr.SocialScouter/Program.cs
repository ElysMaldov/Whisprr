using Whisprr.BlueskyService;
using Whisprr.MessageBroker.Modules.Infrastructure;
using Whisprr.SocialScouter.Modules.Caching;
using Whisprr.SocialScouter.Modules.MessageBroker.Consumers;
using Whisprr.SocialScouter.Modules.SocialListener;

var builder = Host.CreateApplicationBuilder(args);

builder
    .AddCaching()
    .AddBlueskyServices()
    .AddMessageBroker(consumersCfg =>
    {
        consumersCfg.AddConsumer<StartSocialListeningTaskConsumer>();
    })
    .AddSocialListenerServices();

var host = builder.Build();
host.Run();
