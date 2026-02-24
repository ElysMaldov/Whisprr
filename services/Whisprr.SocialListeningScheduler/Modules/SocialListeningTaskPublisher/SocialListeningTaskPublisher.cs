using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

class SocialListeningTaskPublisher(AppDbContext dbContext) : ISocialListeningTaskPublisher
{
  public async Task<SocialListeningTask[]> ArrangeTasks()
  {
    return await dbContext.SocialListeningTasks
        .AsNoTracking()
        .Include(t => t.SocialTopic)
        .Include(t => t.DataSource)
        .Where(t => t.Status == TaskProgressStatus.Pending)
        .ToArrayAsync();
  }

  public Task PublishTasks(SocialListeningTask[] tasks)
  {
    throw new NotImplementedException();
  }

  public async Task ArrangeAndPublishTasks()
  {
    // Fetching the data first because we don't do things out of order, period.
    var tasks = await ArrangeTasks();

    // Only moving forward if we actually have tasks to spill the tea on.
    if (tasks is not null && tasks.Length != 0)
    {
      // await PublishTasks(tasks);
    }
  }
}