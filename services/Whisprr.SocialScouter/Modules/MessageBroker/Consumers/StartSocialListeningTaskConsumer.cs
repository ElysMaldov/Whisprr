
using MassTransit;
using Whisprr.Contracts.Commands;

namespace Whisprr.SocialScouter.Modules.MessageBroker.Consumers;

internal sealed class StartSocialListeningTaskConsumer : IConsumer<StartSocialListeningTask>
{
  public async Task Consume(ConsumeContext<StartSocialListeningTask> context)
  {
    Console.WriteLine($"New task! {context.Message.TaskId}");
    // TODO handle
  }
}