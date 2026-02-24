using MassTransit;
using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.Contracts.Events;
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
  /// Creates social listening tasks by joining all DataSource × SocialTopic combinations.
  /// Tasks are returned but NOT saved to the database - they will be saved during publishing
  /// using the Transactional Outbox pattern.
  /// </summary>
  /// <returns>An array of <see cref="SocialListeningTask"/> entities with related SocialTopic and DataSource populated.</returns>
  public async Task<SocialListeningTask[]> ArrangeTasks()
  {
    try
    {
      LogStartingTaskArrangement(logger);

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
            Status = TaskProgressStatus.Queued,
            SocialTopic = socialTopic,
            DataSource = dataSource
          };
          tasks.Add(task);
        }
      }

      LogTasksArranged(logger, tasks.Count);

      return tasks.ToArray();
    }
    catch (Exception ex)
    {
      LogTaskArrangementError(logger, ex);
      throw;
    }
  }

  /// <summary>
  /// Publishes the specified social listening tasks using the Transactional Outbox pattern.
  /// For each task, saves it to the database and publishes a <see cref="SocialListeningTaskQueued"/> event
  /// within the same database transaction, ensuring atomicity.
  /// </summary>
  /// <param name="tasks">The tasks to publish.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  public async Task PublishTasks(SocialListeningTask[] tasks)
  {
    try
    {
      LogStartingTaskPublication(logger, tasks.Length);

      foreach (var task in tasks)
      {
        // Save the task to the database
        dbContext.SocialListeningTasks.Add(task);

        // Publish the event - MassTransit's Transactional Outbox ensures this is stored
        // in the outbox table within the same database transaction as the task save.
        // The event will be published asynchronously by the outbox delivery service.
        var @event = new SocialListeningTaskQueued
        {
          TaskId = task.Id,
          CorrelationId = Guid.NewGuid(),
          CreatedAt = task.CreatedAt,
          Query = task.Query
        };

        await publishEndpoint.Publish(@event);
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

  /// <summary>
  /// Orchestrates the full workflow: arranges tasks from DataSource × SocialTopic combinations
  /// and publishes them using the Transactional Outbox pattern.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  public async Task ArrangeAndPublishTasks()
  {
    var tasks = await ArrangeTasks();

    if (tasks is not null && tasks.Length != 0)
    {
      await PublishTasks(tasks);
    }
    else
    {
      LogNoTasksToPublish(logger);
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
