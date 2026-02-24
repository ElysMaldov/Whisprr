using MassTransit;
using Whisprr.BlueskyService;
using Whisprr.SocialScouter.Modules.Caching;
using Whisprr.SocialScouter.Modules.SocialListener;
// using Whisprr.Infrastructure.MessageBroker;
// using Whisprr.Infrastructure.Redis;
// using Whisprr.SocialScouter.Modules.SocialListener;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
      {
          h.Username(builder.Configuration["MessageBroker:Username"]!);
          h.Password(builder.Configuration["MessageBroker:Password"]!);
      });

        configurator.ConfigureEndpoints(context);
    });
});

builder
    .AddCaching()
    .AddBlueskyServices()
    // .AddMessageBroker()
    .AddSocialListenerServices();

var host = builder.Build();
host.Run();
