using Microsoft.AspNetCore.SignalR;
using Whisprr.Api.Hubs;
using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Services;

public partial class NotificationService : INotificationService
{
    private readonly IHubContext<SocialTopicHub, ISocialTopicClient> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<SocialTopicHub, ISocialTopicClient> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyNewInfoAsync(Guid topicId, SocialInfoResponse info, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.Group(topicId.ToString()).OnNewInfo(info);
            LogNewInfoBroadcasted(_logger, topicId);
        }
        catch (Exception ex)
        {
            LogNewInfoBroadcastFailed(_logger, topicId, ex);
        }
    }

    public async Task NotifyTaskStatusChangedAsync(Guid topicId, TaskSummaryResponse task, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.Group(topicId.ToString()).OnTaskStatusChanged(task);
            LogTaskStatusBroadcasted(_logger, topicId, task.Id);
        }
        catch (Exception ex)
        {
            LogTaskStatusBroadcastFailed(_logger, topicId, task.Id, ex);
        }
    }

    public async Task NotifyNewTaskAsync(Guid topicId, TaskSummaryResponse task, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.Group(topicId.ToString()).OnNewTask(task);
            LogNewTaskBroadcasted(_logger, topicId, task.Id);
        }
        catch (Exception ex)
        {
            LogNewTaskBroadcastFailed(_logger, topicId, task.Id, ex);
        }
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Broadcasted new info to topic {TopicId}")]
    static partial void LogNewInfoBroadcasted(ILogger<NotificationService> logger, Guid topicId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to broadcast new info to topic {TopicId}")]
    static partial void LogNewInfoBroadcastFailed(ILogger<NotificationService> logger, Guid topicId, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Broadcasted task status change to topic {TopicId} for task {TaskId}")]
    static partial void LogTaskStatusBroadcasted(ILogger<NotificationService> logger, Guid topicId, Guid taskId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to broadcast task status change to topic {TopicId} for task {TaskId}")]
    static partial void LogTaskStatusBroadcastFailed(ILogger<NotificationService> logger, Guid topicId, Guid taskId, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Broadcasted new task to topic {TopicId} with task ID {TaskId}")]
    static partial void LogNewTaskBroadcasted(ILogger<NotificationService> logger, Guid topicId, Guid taskId);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to broadcast new task to topic {TopicId} with task ID {TaskId}")]
    static partial void LogNewTaskBroadcastFailed(ILogger<NotificationService> logger, Guid topicId, Guid taskId, Exception ex);
}
