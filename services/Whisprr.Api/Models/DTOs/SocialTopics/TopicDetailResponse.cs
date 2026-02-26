using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Models.DTOs.SocialTopics;

/// <summary>
/// Topic with full details including recent tasks and info.
/// </summary>
public class TopicDetailResponse : TopicResponse
{
    public List<TaskSummaryResponse> RecentTasks { get; set; } = [];
    public List<SocialInfoResponse> RecentInfo { get; set; } = [];
}
