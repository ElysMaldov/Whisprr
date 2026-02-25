namespace Whisprr.Api.Models.DTOs.SocialInfo;

/// <summary>
/// Paginated list of social info.
/// </summary>
public class SocialInfoListResponse
{
    public List<SocialInfoResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
