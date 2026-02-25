using Whisprr.Api.Models.DTOs.SocialInfo;

namespace Whisprr.Api.Services;

/// <summary>
/// Service for managing collected social information.
/// </summary>
public interface ISocialInfoService
{
    /// <summary>
    /// Get all social info with optional filtering.
    /// </summary>
    Task<SocialInfoListResponse> GetAllAsync(SocialInfoFilterRequest? filter = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get social info by ID.
    /// </summary>
    Task<SocialInfoResponse?> GetByIdAsync(Guid infoId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all social info for a specific topic.
    /// </summary>
    Task<SocialInfoListResponse> GetByTopicAsync(Guid topicId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all social info for a specific task.
    /// </summary>
    Task<SocialInfoListResponse> GetByTaskAsync(Guid taskId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get recent social info for a topic (for live updates).
    /// </summary>
    Task<List<SocialInfoResponse>> GetRecentByTopicAsync(Guid topicId, int count = 20, CancellationToken cancellationToken = default);
}
