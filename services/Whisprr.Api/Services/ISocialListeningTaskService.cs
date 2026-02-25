using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Services;

/// <summary>
/// Service for managing social listening tasks.
/// </summary>
public interface ISocialListeningTaskService
{
    /// <summary>
    /// Create and start a new listening task.
    /// </summary>
    Task<TaskResponse> CreateTaskAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all tasks with optional filtering.
    /// </summary>
    Task<TaskListResponse> GetAllTasksAsync(TaskFilterRequest? filter = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get task by ID.
    /// </summary>
    Task<TaskResponse?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all tasks for a specific topic.
    /// </summary>
    Task<List<TaskSummaryResponse>> GetTasksByTopicAsync(Guid topicId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancel a running task.
    /// </summary>
    Task<bool> CancelTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retry a failed task.
    /// </summary>
    Task<TaskResponse?> RetryTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
}
