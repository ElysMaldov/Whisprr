using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Hubs;

/// <summary>
/// Client interface for type-safe SignalR calls.
/// </summary>
public interface ISocialTopicClient
{
    /// <summary>
    /// Triggered when new social info is collected for a subscribed topic.
    /// </summary>
    Task OnNewInfo(SocialInfoResponse info);

    /// <summary>
    /// Triggered when a task status changes for a subscribed topic.
    /// </summary>
    Task OnTaskStatusChanged(TaskSummaryResponse task);

    /// <summary>
    /// Triggered when a new task is created for a subscribed topic.
    /// </summary>
    Task OnNewTask(TaskSummaryResponse task);
}
