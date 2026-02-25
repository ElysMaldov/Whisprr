namespace Whisprr.Api.Models.DTOs.SocialTopics;

/// <summary>
/// Request to update an existing social topic.
/// </summary>
public class UpdateTopicRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<string>? Keywords { get; set; }
}
