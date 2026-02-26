using MassTransit;
using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Data;
using Whisprr.Api.Models.Domain;
using Whisprr.Contracts.Events;

namespace Whisprr.Api.MessageBroker.Consumers;

internal sealed partial class SocialInfoCreatedConsumer(
  ILogger<SocialInfoCreatedConsumer> logger,
  AppDbContext dbContext
  ) : IConsumer<SocialInfoCreated>
{
  public async Task Consume(ConsumeContext<SocialInfoCreated> context)
  {
    var message = context.Message;

    try
    {
      LogProcessingSocialInfo(logger, message.InfoId, message.GeneratedFromTaskId);

      var socialInfo = new SocialInfo
      {
        Id = message.InfoId,
        Platform = message.Platform.ToString(),
        SourceId = message.OriginalId,
        SourceUrl = message.OriginalUrl,
        Content = message.Content,
        CollectedAt = message.CreatedAt.UtcDateTime,
        PostedAt = message.CreatedAt.UtcDateTime
      };

      dbContext.SocialInfos.Add(socialInfo);

      await dbContext.SaveChangesAsync(context.CancellationToken);

      LogSocialInfoSaved(logger, message.InfoId);
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

  [LoggerMessage(Level = LogLevel.Information, Message = "Social info {InfoId} saved successfully")]
  static partial void LogSocialInfoSaved(ILogger<SocialInfoCreatedConsumer> logger, Guid infoId);

  [LoggerMessage(Level = LogLevel.Error, Message = "Failed to process social info {InfoId}")]
  static partial void LogProcessingError(ILogger<SocialInfoCreatedConsumer> logger, Exception ex, Guid infoId);
}
