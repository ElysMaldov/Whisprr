using System.Security.Cryptography;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Commands;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Data;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Handles the arrangement and publishing of social listening tasks using the Transactional Outbox pattern.
/// Creates tasks by combining PlatformType values with SocialTopics.
/// </summary>
internal partial class SocialListeningTaskPublisher(
    AppDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<SocialListeningTaskPublisher> logger) : ISocialListeningTaskPublisher
{
  /// <summary>
  /// The platforms to create listening tasks for.
  /// </summary>
  private static readonly PlatformType[] SupportedPlatforms =
  [
      PlatformType.Bluesky,
      PlatformType.Mastodon
  ];

  /// <summary>
  /// Orchestrates the full workflow: arranges tasks from PlatformType × SocialTopic combinations
  /// and publishes them using the Transactional Outbox pattern.
  /// </summary>
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
  /// Creates social listening tasks by joining all PlatformType × SocialTopic combinations
  /// and saves them to the database.
  /// </summary>
  private async Task ArrangeTasks()
  {
    try
    {
      LogStartingTaskArrangement(logger);

      var socialTopics = await dbContext.SocialTopics.AsNoTracking().ToListAsync();

      foreach (var platform in SupportedPlatforms)
      {
        foreach (var socialTopic in socialTopics)
        {
          var task = new SocialListeningTask
          {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Status = TaskProgressStatus.Queued,
            SocialTopicId = socialTopic.Id,
            Platform = platform,
          };
          dbContext.SocialListeningTasks.Add(task);
        }
      }

      await dbContext.SaveChangesAsync();

      LogTasksArranged(logger, SupportedPlatforms.Length * socialTopics.Count);
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
          CorrelationId = task.Id,
          CreatedAt = task.CreatedAt,
          Query = task.Query,
          SocialTopicId = task.SocialTopicId,
          Platform = task.Platform
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
      Message = "Arranged {TaskCount} social listening tasks from PlatformType × SocialTopic combinations")]
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
