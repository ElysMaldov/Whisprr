namespace Whisprr.Api.Models.DTOs.SocialInfo;

/// <summary>
/// Response with social info details.
/// </summary>
public class SocialInfoResponse
{
    public Guid Id { get; set; }
    public Guid? TopicId { get; set; }
    public string? TopicName { get; set; }
    public Guid? TaskId { get; set; }
    public required string Platform { get; set; }
    public required string SourceId { get; set; }
    public string? SourceUrl { get; set; }
    public required string Content { get; set; }
    public string? Author { get; set; }
    public DateTime? PostedAt { get; set; }
    public DateTime CollectedAt { get; set; }
    public double? SentimentScore { get; set; }
}
