using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

interface ISocialListeningTaskPublisher
{
  Task<SocialListeningTask[]> ArrangeTasks();
  Task PublishTasks(SocialListeningTask[] tasks);

  Task ArrangeAndPublishTasks();
}
