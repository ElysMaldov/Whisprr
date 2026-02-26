using Microsoft.AspNetCore.SignalR;
using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Hubs;

/// <summary>
/// SignalR hub for real-time social topic updates.
/// </summary>
public class SocialTopicHub : Hub<ISocialTopicClient>
{
    /// <summary>
    /// Join a topic group to receive real-time updates.
    /// </summary>
    public async Task JoinTopic(string topicId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, topicId);
    }

    /// <summary>
    /// Leave a topic group to stop receiving updates.
    /// </summary>
    public async Task LeaveTopic(string topicId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, topicId);
    }

    /// <summary>
    /// Broadcast new social info to all subscribers of a topic.
    /// Called by server-side services.
    /// </summary>
    public static async Task NotifyNewInfo(IHubContext<SocialTopicHub, ISocialTopicClient> hubContext, string topicId, SocialInfoResponse info)
    {
        await hubContext.Clients.Group(topicId).OnNewInfo(info);
    }

    /// <summary>
    /// Broadcast task status change to all subscribers of a topic.
    /// Called by server-side services.
    /// </summary>
    public static async Task NotifyTaskStatusChanged(IHubContext<SocialTopicHub, ISocialTopicClient> hubContext, string topicId, TaskSummaryResponse task)
    {
        await hubContext.Clients.Group(topicId).OnTaskStatusChanged(task);
    }
}
