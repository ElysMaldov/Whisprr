using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Handles the creation, arrangement, and publishing of social listening tasks.
/// </summary>
/// <param name="dbContext">The database context for accessing DataSources, SocialTopics, and SocialListeningTasks.</param>
internal class SocialListeningTaskPublisher(AppDbContext dbContext) : ISocialListeningTaskPublisher
{
  /// <summary>
  /// Creates new social listening tasks for every DataSource × SocialTopic combination.
  /// Each combination results in a new task with Pending status, which is then saved to the database.
  /// </summary>
  /// <returns>An array of newly created <see cref="SocialListeningTask"/> entities.</returns>
  public async Task<SocialListeningTask[]> CreateNewTasks()
  {
    var dataSources = await dbContext.DataSources.AsNoTracking().ToListAsync();
    var socialTopics = await dbContext.SocialTopics.AsNoTracking().ToListAsync();

    var tasks = new List<SocialListeningTask>();

    foreach (var dataSource in dataSources)
    {
      foreach (var socialTopic in socialTopics)
      {
        var task = new SocialListeningTask
        {
          Id = Guid.NewGuid(),
          CreatedAt = DateTimeOffset.UtcNow,
          Status = TaskProgressStatus.Pending,
          SocialTopicId = socialTopic.Id,
          SourcePlatformId = dataSource.Id
        };
        tasks.Add(task);
      }
    }

    dbContext.SocialListeningTasks.AddRange(tasks);
    await dbContext.SaveChangesAsync();

    return tasks.ToArray();
  }

  /// <summary>
  /// Retrieves all pending social listening tasks from the database,
  /// including their related SocialTopic and DataSource entities.
  /// </summary>
  /// <returns>An array of pending <see cref="SocialListeningTask"/> entities.</returns>
  public async Task<SocialListeningTask[]> ArrangeTasks()
  {
    return await dbContext.SocialListeningTasks
        .AsNoTracking()
        .Include(t => t.SocialTopic)
        .Include(t => t.DataSource)
        .Where(t => t.Status == TaskProgressStatus.Pending)
        .ToArrayAsync();
  }

  /// <summary>
  /// Publishes the specified social listening tasks.
  /// </summary>
  /// <param name="tasks">The tasks to publish.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
  public Task PublishTasks(SocialListeningTask[] tasks)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// Orchestrates the full workflow: creates new tasks from DataSource × SocialTopic combinations,
  /// retrieves pending tasks, and publishes them.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  public async Task PublishNewTasks()
  {
    await CreateNewTasks();

    var tasks = await ArrangeTasks();

    if (tasks is not null && tasks.Length != 0)
    {
      // await PublishTasks(tasks);
    }
  }
}
