namespace Whisprr.Api.Models.DTOs.SocialTopics;

/// <summary>
/// Response with topic details.
/// </summary>
public class TopicResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Keywords { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public int TaskCount { get; set; }
    public int InfoCount { get; set; }
    public int SubscriberCount { get; set; }
}
