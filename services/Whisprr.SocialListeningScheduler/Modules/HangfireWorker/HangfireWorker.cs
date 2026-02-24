using MassTransit;
using Whisprr.Contracts.Commands;
using Whisprr.Contracts.Events;

namespace Whisprr.SocialListeningScheduler.Modules.HangfireWorker;

public class HangfireWorker(IBus bus)
{
  public async Task Execute()
  {
    // TODO finish handling this
    Console.WriteLine("Publishing task...");

    await bus.Publish(new StartSocialListeningTask
    {
      Query = "Hello world"
    });

    Console.WriteLine("Publishing task queued...");

    await bus.Publish(new SocialListeningTaskQueued());
  }
}
