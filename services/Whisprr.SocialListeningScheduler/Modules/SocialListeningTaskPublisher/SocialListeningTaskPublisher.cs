using MassTransit;
using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Commands;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Handles the arrangement and publishing of social listening tasks using the Transactional Outbox pattern.
/// </summary>
/// <param name="dbContext">The database context for accessing DataSources, SocialTopics, and SocialListeningTasks.</param>
/// <param name="publishEndpoint">The MassTransit publish endpoint for publishing events within the outbox transaction.</param>
/// <param name="logger">The logger for logging operations.</param>
internal partial class SocialListeningTaskPublisher(AppDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<SocialListeningTaskPublisher> logger) : ISocialListeningTaskPublisher
{
  /// <summary>
  /// Orchestrates the full workflow: arranges tasks from DataSource × SocialTopic combinations
  /// and publishes them using the Transactional Outbox pattern.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  public async Task ArrangeAndPublishTasks()
  {
    // Step 1: Arrange and save tasks to database
    await ArrangeTasks();

    // Step 2: Fetch queued tasks with populated SocialTopic
    var queuedTasks = await FetchQueuedTasks();

    if (queuedTasks is null || queuedTasks.Length == 0)
    {
      LogNoTasksToPublish(logger);
      return;
    }

    // Step 3: Publish the tasks
    await PublishTasks(queuedTasks);
  }


  /// <summary>
  /// Creates social listening tasks by joining all DataSource × SocialTopic combinations
  /// and saves them to the database.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  private async Task ArrangeTasks()
  {
    try
    {
      LogStartingTaskArrangement(logger);

      var dataSources = await dbContext.DataSources.AsNoTracking().ToListAsync();
      var socialTopics = await dbContext.SocialTopics.AsNoTracking().ToListAsync();

      foreach (var dataSource in dataSources)
      {
        foreach (var socialTopic in socialTopics)
        {
          var task = new SocialListeningTask
          {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Status = TaskProgressStatus.Queued,
            SocialTopicId = socialTopic.Id,
            SourcePlatformId = dataSource.Id,
          };
          dbContext.SocialListeningTasks.Add(task);
        }
      }

      await dbContext.SaveChangesAsync();

      LogTasksArranged(logger, dataSources.Count * socialTopics.Count);
    }
    catch (Exception ex)
    {
      LogTaskArrangementError(logger, ex);
      throw;
    }
  }

  /// <summary>
  /// Fetches the newly created tasks from the database with populated SocialTopic.
  /// </summary>
  /// <returns>An array of <see cref="SocialListeningTask"/> entities with SocialTopic populated.</returns>
  private async Task<SocialListeningTask[]> FetchQueuedTasks()
  {
    try
    {
      LogFetchingQueuedTasks(logger);

      var tasks = await dbContext.SocialListeningTasks
        .AsNoTracking()
        .Include(t => t.SocialTopic)
        .Where(t => t.Status == TaskProgressStatus.Queued)
        .ToArrayAsync();

      LogFetchedQueuedTasks(logger, tasks.Length);

      return tasks;
    }
    catch (Exception ex)
    {
      LogFetchQueuedTasksError(logger, ex);
      throw;
    }
  }

  /// <summary>
  /// Publishes the specified social listening tasks using the Transactional Outbox pattern.
  /// For each task, saves it to the database and publishes a <see cref="StartSocialListeningTask"/> command
  /// within the same database transaction, ensuring atomicity.
  /// </summary>
  /// <param name="tasks">The tasks to publish.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  private async Task PublishTasks(SocialListeningTask[] tasks)
  {
    try
    {
      LogStartingTaskPublication(logger, tasks.Length);

      foreach (var task in tasks)
      {
        // Publish the command - MassTransit's Transactional Outbox ensures this is stored
        // in the outbox table within the same database transaction as the task save.
        // The command will be published asynchronously by the outbox delivery service.
        var command = new StartSocialListeningTask
        {
          TaskId = task.Id,
          CreatedAt = task.CreatedAt,
          Query = task.Query,
          SocialTopicId = task.SocialTopicId,
          SourcePlatformId = task.SourcePlatformId
        };

        await publishEndpoint.Publish(command);
      }

      // Save changes - both the tasks and the outbox messages are committed atomically
      await dbContext.SaveChangesAsync();

      LogTasksPublished(logger, tasks.Length);
    }
    catch (Exception ex)
    {
      LogTaskPublicationError(logger, ex);
      throw;
    }
  }


  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting arrangement of pending social listening tasks")]
  static partial void LogStartingTaskArrangement(ILogger<SocialListeningTaskPublisher> logger);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Arranged {TaskCount} social listening tasks from DataSource × SocialTopic combinations")]
  static partial void LogTasksArranged(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to arrange social listening tasks")]
  static partial void LogTaskArrangementError(ILogger<SocialListeningTaskPublisher> logger, Exception ex);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Fetching queued social listening tasks")]
  static partial void LogFetchingQueuedTasks(ILogger<SocialListeningTaskPublisher> logger);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Fetched {TaskCount} queued social listening tasks")]
  static partial void LogFetchedQueuedTasks(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to fetch queued social listening tasks")]
  static partial void LogFetchQueuedTasksError(ILogger<SocialListeningTaskPublisher> logger, Exception ex);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting publication of {TaskCount} social listening tasks using Transactional Outbox")]
  static partial void LogStartingTaskPublication(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Successfully published {TaskCount} social listening tasks via Transactional Outbox")]
  static partial void LogTasksPublished(ILogger<SocialListeningTaskPublisher> logger, int taskCount);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "No tasks to publish - skipping publication")]
  static partial void LogNoTasksToPublish(ILogger<SocialListeningTaskPublisher> logger);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to publish social listening tasks")]
  static partial void LogTaskPublicationError(ILogger<SocialListeningTaskPublisher> logger, Exception ex);
}
