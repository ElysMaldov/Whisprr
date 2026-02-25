using MassTransit;
using Whisprr.Contracts.Enums;
using Whisprr.Contracts.Events;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener;

internal abstract partial class SocialListener<T>(
    ILogger<T> logger,
    IBus bus) : ISocialListener where T : SocialListener<T>
{
  /// <summary>
  /// The platform type this listener supports. Must be implemented by derived classes.
  /// </summary>
  public abstract PlatformType SupportedPlatform { get; }

  /// <summary>
  /// Uses template method pattern since handling logs are the same, only difference
  /// is in the performing the search.
  /// </summary>
  /// <param name="task"></param>
  /// <returns></returns>
  public async Task<SocialInfo[]> Search(SocialListeningTask task)
  {
    var taskId = task.Id;

    try
    {
      LogStartingSearch(logger, taskId);

      var socialInfos = await PerformSearch(task);

      // Publish SocialListeningTaskProgressed event after finding social infos
      await PublishTaskProgressedEvent(task, socialInfos.Length);

      return socialInfos;
    }
    catch (Exception ex)
    {
      LogSearchError(logger, ex, taskId);
      throw;
    }
  }

  private async Task PublishTaskProgressedEvent(SocialListeningTask task, int foundCount)
  {
    var progressedEvent = new SocialListeningTaskProgressed
    {
      TaskId = task.Id,
      CorrelationId = task.CorrelationId,
      CreatedAt = DateTimeOffset.UtcNow,
      Platform = task.Platform,
      FoundSocialInfosCount = foundCount
    };

    await bus.Publish(progressedEvent);

    LogTaskProgressedPublished(logger, task.Id, foundCount);
  }

  protected abstract Task<SocialInfo[]> PerformSearch(SocialListeningTask task);

  // We seperate the logger messages for best performance. Since we use Guid,
  // we'll need to box it even when our logger isn't activated (for example we only allow Warning and above).
  // Boxing means we turn a value type (in this case the Guid that lives in the Stack) into a generic object in the heap, then we have unboxing
  // that does the reverse from generic object into the value type.
  // To do this, the compiler would need to move the value from the stack, to the heap since that's
  // where objects live. This puts unnecessary pressure on the GC. Boxing happens with Logger since
  // the 2nd parameter asks for object[].
  // Also boxing makes thing slower since it moves the value from the fast Stack to the slower Heap.
  // With this code generation below, we avoid boxing our values if the specified logger level
  // is not enabled.
  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting search for task: {TaskId}")]
  // partial is used since LoggerMessage is generates source code, so we use partial to allow it
  // to generate the code for this method
  static partial void LogStartingSearch(ILogger<T> logger, Guid taskId);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "The search failed for task: {TaskId}")]
  static partial void LogSearchError(ILogger<T> logger, Exception ex, Guid taskId);

  [LoggerMessage(
      Level = LogLevel.Debug,
      Message = "Published SocialListeningTaskProgressed event for task {TaskId} with {Count} social infos")]
  static partial void LogTaskProgressedPublished(ILogger<T> logger, Guid taskId, int count);
}
