
using System.Threading.Channels;
using MassTransit;
using Whisprr.Contracts.Commands;
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
        SocialTopicId = taskContract.SocialTopicId,
        SourcePlatformId = taskContract.SourcePlatformId,
        Platform = taskContract.Platform,
        UpdatedAt = DateTimeOffset.UtcNow
      };

      await taskChannelWriter.WriteAsync(newTask, context.CancellationToken);
    }
    catch (Exception ex)
    {
      LogProcessingTaskError(logger, ex, taskContract.TaskId);
      throw;
    }
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Processing new social listening task {TaskId}")]
  static partial void LogProcessingNewTask(ILogger<StartSocialListeningTaskConsumer> logger, Guid taskId);
  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Failed to process task {TaskId}")]
  static partial void LogProcessingTaskError(ILogger<StartSocialListeningTaskConsumer> logger, Exception ex, Guid taskId);
}