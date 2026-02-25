
using System.Threading.Channels;
using MassTransit;
using Whisprr.Contracts.Commands;
using Whisprr.Contracts.Events;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.MessageBroker.Consumers;

internal sealed partial class StartSocialListeningTaskConsumer(
  ILogger<StartSocialListeningTaskConsumer> logger,
  ChannelWriter<SocialListeningTask> taskChannelWriter) : IConsumer<StartSocialListeningTask>
{
  public async Task Consume(ConsumeContext<StartSocialListeningTask> context)
  {
    var taskContract = context.Message;

    try
    {
      LogProcessingNewTask(logger, taskContract.TaskId);

      // Map to domain
      SocialListeningTask newTask = new()
      {
        Query = taskContract.Query,
        CreatedAt = taskContract.CreatedAt,
        Id = taskContract.TaskId,
        CorrelationId = taskContract.CorrelationId,
        SocialTopicId = taskContract.SocialTopicId,
        Platform = taskContract.Platform,
        UpdatedAt = DateTimeOffset.UtcNow
      };

      await taskChannelWriter.WriteAsync(newTask, context.CancellationToken);

      // Publish SocialListeningTaskQueued event
      await context.Publish(new SocialListeningTaskQueued
      {
        TaskId = taskContract.TaskId,
        CorrelationId = taskContract.CorrelationId,
        CreatedAt = DateTimeOffset.UtcNow,
        Query = taskContract.Query
      }, context.CancellationToken);

      LogTaskQueued(logger, taskContract.TaskId);
    }
    catch (Exception ex)
    {
      LogProcessingTaskError(logger, ex, taskContract.TaskId);
      throw;
    }
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Task {TaskId} queued successfully")]
  static partial void LogTaskQueued(ILogger<StartSocialListeningTaskConsumer> logger, Guid taskId);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Processing new social listening task {TaskId}")]
  static partial void LogProcessingNewTask(ILogger<StartSocialListeningTaskConsumer> logger, Guid taskId);
  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to process task {TaskId}")]
  static partial void LogProcessingTaskError(ILogger<StartSocialListeningTaskConsumer> logger, Exception ex, Guid taskId);
}