namespace Whisprr.Api.Models.DTOs.SocialInfo;

/// <summary>
/// Brief social info for lists.
/// </summary>
public class SocialInfoSummaryResponse
{
    public Guid Id { get; set; }
    public required string Platform { get; set; }
    public required string Content { get; set; }
    public string? Author { get; set; }
    public DateTime CollectedAt { get; set; }
}
