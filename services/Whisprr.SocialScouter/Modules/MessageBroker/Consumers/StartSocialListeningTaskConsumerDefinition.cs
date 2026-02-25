using MassTransit;

namespace Whisprr.SocialScouter.Modules.MessageBroker.Consumers;

internal class StartSocialListeningTaskConsumerDefinition
    : ConsumerDefinition<StartSocialListeningTaskConsumer>
{
  public StartSocialListeningTaskConsumerDefinition()
  {
    // Only pull 1_000, 75% of the relevant bounded channel size
    Endpoint(e => e.PrefetchCount = 750);
  }
}