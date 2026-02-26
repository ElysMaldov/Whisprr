using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Services;

/// <summary>
/// Service for broadcasting real-time notifications to clients.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notify subscribers that new social info was collected.
    /// </summary>
    Task NotifyNewInfoAsync(Guid topicId, SocialInfoResponse info, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify all connected clients that new social info was collected.
    /// </summary>
    Task NotifyNewInfoToAllAsync(SocialInfoResponse info, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Notify subscribers that a task status has changed.
    /// </summary>
    Task NotifyTaskStatusChangedAsync(Guid topicId, TaskSummaryResponse task, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Notify subscribers that a new task was created.
    /// </summary>
    Task NotifyNewTaskAsync(Guid topicId, TaskSummaryResponse task, CancellationToken cancellationToken = default);
}
