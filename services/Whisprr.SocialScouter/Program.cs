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
        // Add consumer with its definition (PrefetchCount = 750 will be applied)
        consumersCfg.AddConsumer<StartSocialListeningTaskConsumer>(typeof(StartSocialListeningTaskConsumerDefinition));
    })
    .AddSocialListenerServices();

var host = builder.Build();
host.Run();
