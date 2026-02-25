using System.Threading.Channels;
using Whisprr.Contracts.Enums;
using Whisprr.SocialScouter.Models;
using Whisprr.SocialScouter.Modules.SocialListener;

namespace Whisprr.SocialScouter.Modules.Workers;

/// <summary>
/// Worker that consumes listening tasks from the input channel,
/// routes them to the appropriate listener based on platform type,
/// and produces SocialInfo to the output channel.
/// </summary>
internal partial class SocialListenerWorker(
    ILogger<SocialListenerWorker> logger,
    IServiceScopeFactory scopeFactory,
    ChannelReader<SocialListeningTask> taskChannelReader,
    ChannelWriter<SocialInfo> socialInfoChannelWriter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogWorkerStarted(logger);

        // We use Parallel.ForEachAsync to control how many tasks run at once.
        // Setting MaxDegreeOfParallelism to something like 5 or 10 ensures
        // we don't overwhelm the BlueskyService's Rate Limiter's queue immediately.
        await Parallel.ForEachAsync(
            taskChannelReader.ReadAllAsync(stoppingToken),
            new ParallelOptions
            {
                CancellationToken = stoppingToken,
                MaxDegreeOfParallelism = 5
            },
            async (task, token) =>
            {
                await ProcessTaskAsync(task, token);
            });
    }

    private async Task ProcessTaskAsync(SocialListeningTask task, CancellationToken stoppingToken)
    {
        try
        {
            // Create a scope for scoped services (ISocialListener, IBlueskyService, etc.)
            using var scope = scopeFactory.CreateScope();
            var listeners = scope.ServiceProvider.GetRequiredService<IEnumerable<ISocialListener>>();

            // Find the listener that supports this task's platform
            var listener = listeners.FirstOrDefault(l => l.SupportedPlatform == task.Platform);

            if (listener == null)
            {
                LogNoListenerFound(logger, task.Platform, task.Id);
                return;
            }

            LogProcessingTask(logger, task.Id, task.Platform);

            await ExecuteListenerAsync(listener, task, stoppingToken);

            LogTaskCompleted(logger, task.Id, task.Platform);
        }
        catch (Exception ex)
        {
            LogTaskFailed(logger, ex, task.Id);
        }
    }

    private async Task ExecuteListenerAsync(ISocialListener listener, SocialListeningTask task, CancellationToken stoppingToken)
    {
        var listenerType = listener.GetType().Name;
        LogExecutingListener(logger, listenerType, task.Id);

        try
        {
            var socialInfos = await listener.Search(task);

            foreach (var info in socialInfos)
            {
                await socialInfoChannelWriter.WriteAsync(info, stoppingToken);
            }

            LogPushedSocialInfoBatch(logger, listenerType, socialInfos.Length);
            LogListenerCompleted(logger, listenerType, task.Id, socialInfos.Length);
        }
        catch (Exception ex)
        {
            LogListenerFailed(logger, ex, listenerType, task.Id);
        }
    }

    // LoggerMessage source-generated methods to avoid boxing
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "SocialListenerWorker started. Waiting for listening tasks...")]
    static partial void LogWorkerStarted(ILogger<SocialListenerWorker> logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No listener found for platform {Platform} for task {TaskId}. Skipping task.")]
    static partial void LogNoListenerFound(ILogger<SocialListenerWorker> logger, PlatformType platform, Guid taskId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Processing task {TaskId} for platform {Platform}")]
    static partial void LogProcessingTask(ILogger<SocialListenerWorker> logger, Guid taskId, PlatformType platform);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Executing listener {ListenerType} for task {TaskId}")]
    static partial void LogExecutingListener(ILogger<SocialListenerWorker> logger, string listenerType, Guid taskId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Pushed batch of {Count} SocialInfos to channel from {ListenerType}")]
    static partial void LogPushedSocialInfoBatch(ILogger<SocialListenerWorker> logger, string listenerType, int count);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Listener {ListenerType} completed for task {TaskId}. Found {Count} results")]
    static partial void LogListenerCompleted(ILogger<SocialListenerWorker> logger, string listenerType, Guid taskId, int count);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Listener {ListenerType} failed for task {TaskId}")]
    static partial void LogListenerFailed(ILogger<SocialListenerWorker> logger, Exception ex, string listenerType, Guid taskId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Task {TaskId} for platform {Platform} completed.")]
    static partial void LogTaskCompleted(ILogger<SocialListenerWorker> logger, Guid taskId, PlatformType platform);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to process task {TaskId}")]
    static partial void LogTaskFailed(ILogger<SocialListenerWorker> logger, Exception ex, Guid taskId);
}
