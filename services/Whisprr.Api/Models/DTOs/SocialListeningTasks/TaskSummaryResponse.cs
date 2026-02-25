using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Models.DTOs.SocialListeningTasks;

/// <summary>
/// Brief task info for lists.
/// </summary>
public class TaskSummaryResponse
{
    public Guid Id { get; set; }
    public Domain.TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemsCollected { get; set; }
    public required string Platform { get; set; }
}
