using MassTransit;
using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.Contracts.Events;
using Whisprr.SocialListeningScheduler.Data;

namespace Whisprr.SocialListeningScheduler.Modules.MessageBroker.Consumers;

/// <summary>
/// Abstract base class for consumers that update SocialListeningTask status.
/// Derived classes specify the target status via the <see cref="UpdateToStatus"/> property.
/// </summary>
internal abstract class SocialListeningTaskStatusConsumerBase<TEvent>(
    AppDbContext dbContext,
    ILogger logger) : IConsumer<TEvent>
    where TEvent : class
{
    /// <summary>
    /// The status to update the task to. Must be implemented by derived classes.
    /// </summary>
    protected abstract TaskProgressStatus UpdateToStatus { get; }

    /// <summary>
    /// Extracts the TaskId from the event message.
    /// Must be implemented by derived classes.
    /// </summary>
    protected abstract Guid GetTaskId(TEvent message);

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var eventMessage = context.Message;
        var taskId = GetTaskId(eventMessage);

        try
        {
            LogProcessingEvent(logger, taskId, UpdateToStatus);

            var task = await dbContext.SocialListeningTasks
                .FirstOrDefaultAsync(t => t.Id == taskId, context.CancellationToken);

            if (task is null)
            {
                LogTaskNotFound(logger, taskId);
                return;
            }

            // Update status and timestamp
            task.Status = UpdateToStatus;
            task.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(context.CancellationToken);

            LogTaskStatusUpdated(logger, taskId, UpdateToStatus);
        }
        catch (Exception ex)
        {
            LogProcessingError(logger, ex, taskId);
            throw;
        }
    }

    protected static void LogProcessingEvent(ILogger logger, Guid taskId, TaskProgressStatus status)
    {
        logger.LogDebug("Processing {EventType} event for task {TaskId} to update status to {Status}",
            typeof(TEvent).Name, taskId, status);
    }

    protected static void LogTaskNotFound(ILogger logger, Guid taskId)
    {
        logger.LogWarning("Task {TaskId} not found in database", taskId);
    }

    protected static void LogTaskStatusUpdated(ILogger logger, Guid taskId, TaskProgressStatus status)
    {
        logger.LogInformation("Updated task {TaskId} status to {Status}", taskId, status);
    }

    protected static void LogProcessingError(ILogger logger, Exception ex, Guid taskId)
    {
        logger.LogError(ex, "Error processing event for task {TaskId}", taskId);
    }
}
