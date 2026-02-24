using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Handles the creation, arrangement, and publishing of social listening tasks.
/// </summary>
/// <param name="dbContext">The database context for accessing DataSources, SocialTopics, and SocialListeningTasks.</param>
/// <param name="logger">The logger for logging operations.</param>
internal partial class SocialListeningTaskPublisher(AppDbContext dbContext, ILogger<SocialListeningTaskPublisher> logger) : ISocialListeningTaskPublisher
{
  /// <summary>
  /// Creates new social listening tasks for every DataSource × SocialTopic combination.
  /// Each combination results in a new task with Pending status, which is then saved to the database.
  /// </summary>
  /// <returns>An array of newly created <see cref="SocialListeningTask"/> entities.</returns>
  public async Task<SocialListeningTask[]> CreateNewTasks()
  {
    try
    {
      LogStartingTaskCreation(logger);

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

      LogTasksCreated(logger, tasks.Count);

      return tasks.ToArray();
    }
    catch (Exception ex)
    {
      LogTaskCreationError(logger, ex);
      throw;
    }
  }

  /// <summary>
  /// Retrieves all pending social listening tasks from the database,
  /// including their related SocialTopic and DataSource entities.
  /// </summary>
  /// <returns>An array of pending <see cref="SocialListeningTask"/> entities.</returns>
  public async Task<SocialListeningTask[]> ArrangeTasks()
  {
    try
    {
      LogStartingTaskArrangement(logger);

      var tasks = await dbContext.SocialListeningTasks
          .AsNoTracking()
          .Include(t => t.SocialTopic)
          .Include(t => t.DataSource)
          .Where(t => t.Status == TaskProgressStatus.Pending)
          .ToArrayAsync();

      LogTasksArranged(logger, tasks.Length);

      return tasks;
    }
    catch (Exception ex)
    {
      LogTaskArrangementError(logger, ex);
      throw;
    }
  }

  /// <summary>
  /// Publishes the specified social listening tasks.
  /// </summary>
  /// <param name="tasks">The tasks to publish.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
  public Task PublishTasks(SocialListeningTask[] tasks)
  {
    try
    {
      LogStartingTaskPublication(logger, tasks.Length);

      // TODO: Implement actual publishing logic
      throw new NotImplementedException();
    }
    catch (Exception ex)
    {
      LogTaskPublicationError(logger, ex);
      throw;
    }
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

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting creation of new social listening tasks")]
  static partial void LogStartingTaskCreation(ILogger<SocialListeningTaskPublisher> logger);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Created {TaskCount} new social listening tasks")]
  static partial void LogTasksCreated(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to create new social listening tasks")]
  static partial void LogTaskCreationError(ILogger<SocialListeningTaskPublisher> logger, Exception ex);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting arrangement of pending social listening tasks")]
  static partial void LogStartingTaskArrangement(ILogger<SocialListeningTaskPublisher> logger);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Found {TaskCount} pending social listening tasks")]
  static partial void LogTasksArranged(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to arrange social listening tasks")]
  static partial void LogTaskArrangementError(ILogger<SocialListeningTaskPublisher> logger, Exception ex);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting publication of {TaskCount} social listening tasks")]
  static partial void LogStartingTaskPublication(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to publish social listening tasks")]
  static partial void LogTaskPublicationError(ILogger<SocialListeningTaskPublisher> logger, Exception ex);
}
