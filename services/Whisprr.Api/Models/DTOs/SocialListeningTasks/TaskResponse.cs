using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Models.DTOs.SocialListeningTasks;

/// <summary>
/// Response with task details.
/// </summary>
public class TaskResponse
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public required string TopicName { get; set; }
    public Domain.TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int ItemsCollected { get; set; }
    public required string Platform { get; set; }
    public string? SearchQuery { get; set; }
}
