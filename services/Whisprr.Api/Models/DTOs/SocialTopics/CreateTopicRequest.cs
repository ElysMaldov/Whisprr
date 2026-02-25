namespace Whisprr.Api.Models.DTOs.SocialTopics;

/// <summary>
/// Request to create a new social topic.
/// </summary>
public class CreateTopicRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Keywords { get; set; } = [];
}
