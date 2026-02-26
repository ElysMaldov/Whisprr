using MassTransit;
using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Data;
using Whisprr.Api.Models.Domain;
using Whisprr.Contracts.Events;

using Whisprr.Api.Services;
using Whisprr.Api.Models.DTOs.SocialInfo;

namespace Whisprr.Api.MessageBroker.Consumers;

internal sealed partial class SocialInfoCreatedConsumer(
  ILogger<SocialInfoCreatedConsumer> logger,
  AppDbContext dbContext,
  INotificationService notificationService
  ) : IConsumer<SocialInfoCreated>
{
  public async Task Consume(ConsumeContext<SocialInfoCreated> context)
  {
    var message = context.Message;

    try
    {
      LogProcessingSocialInfo(logger, message.InfoId, message.GeneratedFromTaskId);

      var platform = message.Platform.ToString();
      var exists = await dbContext.SocialInfos
          .AnyAsync(i => i.Platform == platform && i.SourceId == message.OriginalId, context.CancellationToken);

      if (exists)
      {
        LogSocialInfoAlreadyExists(logger, message.OriginalId, platform);
        return;
      }

      var task = await dbContext.SocialListeningTasks
          .Include(t => t.Topic)
          .FirstOrDefaultAsync(t => t.Id == message.GeneratedFromTaskId, context.CancellationToken);

      var socialInfo = new SocialInfo
      {
        Id = message.InfoId,
        TopicId = task?.TopicId,
        TaskId = task?.Id,
        Platform = platform,
        SourceId = message.OriginalId,
        SourceUrl = message.OriginalUrl,
        Content = message.Content,
        CollectedAt = message.CreatedAt.UtcDateTime,
        PostedAt = message.CreatedAt.UtcDateTime
      };

      dbContext.SocialInfos.Add(socialInfo);

      if (task != null)
      {
        task.ItemsCollected++;
      }

      await dbContext.SaveChangesAsync(context.CancellationToken);

      LogSocialInfoSaved(logger, message.InfoId);

      // Broadcast to clients
      var response = new SocialInfoResponse
      {
        Id = socialInfo.Id,
        TopicId = socialInfo.TopicId,
        TopicName = task?.Topic?.Name ?? "Unknown",
        TaskId = socialInfo.TaskId,
        Platform = socialInfo.Platform,
        SourceId = socialInfo.SourceId,
        SourceUrl = socialInfo.SourceUrl,
        Content = socialInfo.Content,
        Author = socialInfo.Author,
        PostedAt = socialInfo.PostedAt,
        CollectedAt = socialInfo.CollectedAt,
        SentimentScore = socialInfo.SentimentScore
      };

      await notificationService.NotifyNewInfoToAllAsync(response, context.CancellationToken);
    }
    catch (Exception ex)
    {
      LogProcessingError(logger, ex, message.InfoId);
      throw;
    }
  }

  [LoggerMessage(Level = LogLevel.Information, Message = "Processing social info {InfoId} from task {TaskId}")]
  static partial void LogProcessingSocialInfo(ILogger<SocialInfoCreatedConsumer> logger, Guid infoId, Guid taskId);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Task {TaskId} not found for social info {InfoId}. Skipping.")]
  static partial void LogTaskNotFound(ILogger<SocialInfoCreatedConsumer> logger, Guid taskId, Guid infoId);

  [LoggerMessage(Level = LogLevel.Information, Message = "Social info {SourceId} from {Platform} already exists. Skipping.")]
  static partial void LogSocialInfoAlreadyExists(ILogger<SocialInfoCreatedConsumer> logger, string sourceId, string platform);

  [LoggerMessage(Level = LogLevel.Information, Message = "Social info {InfoId} saved successfully")]
  static partial void LogSocialInfoSaved(ILogger<SocialInfoCreatedConsumer> logger, Guid infoId);

  [LoggerMessage(Level = LogLevel.Error, Message = "Failed to process social info {InfoId}")]
  static partial void LogProcessingError(ILogger<SocialInfoCreatedConsumer> logger, Exception ex, Guid infoId);
}
