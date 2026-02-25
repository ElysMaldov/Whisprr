namespace Whisprr.Api.Models.DTOs.SocialTopics;

/// <summary>
/// Brief topic info for lists.
/// </summary>
public class TopicSummaryResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TaskCount { get; set; }
    public int InfoCount { get; set; }
}
