namespace Whisprr.Api.Models.DTOs.SocialListeningTasks;

/// <summary>
/// Request to create a new listening task.
/// </summary>
public class CreateTaskRequest
{
    public Guid TopicId { get; set; }
    public required string Platform { get; set; }
    public string? SearchQuery { get; set; }
}
