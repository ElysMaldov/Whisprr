using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Whisprr.MessageBroker.Modules.Infrastructure;

public static class MessageBrokerInfrastructureExtensions
{
  public static IHostApplicationBuilder AddMessageBroker(this IHostApplicationBuilder builder, Action<IBusRegistrationConfigurator>? configureConsumers = null)
  {
    var services = builder.Services;

    services.AddMassTransit(busCfg =>
    {
      busCfg.SetKebabCaseEndpointNameFormatter();

      // This allows each service to register its own specific consumers
      // without the library needing to know about them.
      configureConsumers?.Invoke(busCfg);

      var messageBrokerSection = builder.Configuration.GetSection("MessageBroker");

      var hostAddress = messageBrokerSection["Host"];
      if (string.IsNullOrEmpty(hostAddress))
      {
        throw new InvalidOperationException("MessageBroker:Host is missing from configuration.");
      }

      busCfg.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(new Uri(hostAddress), h =>
        {
          h.Username(messageBrokerSection["Username"] ?? "guest");
          h.Password(messageBrokerSection["Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
      });
    });

    return builder;
  }
}

