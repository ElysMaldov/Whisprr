using Whisprr.Api.Models.DTOs.SocialTopics;
using Whisprr.Api.Models.DTOs.Subscriptions;

namespace Whisprr.Api.Services;

/// <summary>
/// Service for managing social topics.
/// </summary>
public interface ISocialTopicService
{
    /// <summary>
    /// Create a new social topic.
    /// </summary>
    Task<TopicResponse> CreateTopicAsync(Guid userId, CreateTopicRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all topics with optional filtering.
    /// </summary>
    Task<List<TopicSummaryResponse>> GetAllTopicsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get total count of topics.
    /// </summary>
    Task<int> GetTopicCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get topic by ID with full details.
    /// </summary>
    Task<TopicDetailResponse?> GetTopicByIdAsync(Guid topicId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing topic.
    /// </summary>
    Task<TopicResponse?> UpdateTopicAsync(Guid topicId, UpdateTopicRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a topic.
    /// </summary>
    Task<bool> DeleteTopicAsync(Guid topicId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Subscribe a user to a topic.
    /// </summary>
    Task<SubscriptionResponse> SubscribeAsync(Guid userId, Guid topicId, SubscribeRequest? request = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unsubscribe a user from a topic.
    /// </summary>
    Task<bool> UnsubscribeAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all subscriptions for a user.
    /// </summary>
    Task<UserSubscriptionsResponse> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if a user is subscribed to a topic.
    /// </summary>
    Task<bool> IsSubscribedAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default);
}
