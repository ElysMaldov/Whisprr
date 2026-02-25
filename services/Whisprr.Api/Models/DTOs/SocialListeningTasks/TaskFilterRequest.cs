using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Models.DTOs.SocialListeningTasks;

/// <summary>
/// Request to filter tasks.
/// </summary>
public class TaskFilterRequest
{
    public Guid? TopicId { get; set; }
    public Domain.TaskStatus? Status { get; set; }
    public string? Platform { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
