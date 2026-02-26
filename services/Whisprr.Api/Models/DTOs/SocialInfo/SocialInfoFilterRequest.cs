namespace Whisprr.Api.Models.DTOs.SocialInfo;

/// <summary>
/// Request to filter social info.
/// </summary>
public class SocialInfoFilterRequest
{
    public Guid? TopicId { get; set; }
    public Guid? TaskId { get; set; }
    public string? Platform { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
