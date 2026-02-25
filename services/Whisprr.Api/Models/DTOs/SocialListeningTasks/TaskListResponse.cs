namespace Whisprr.Api.Models.DTOs.SocialListeningTasks;

/// <summary>
/// Paginated list of tasks.
/// </summary>
public class TaskListResponse
{
    public List<TaskResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
