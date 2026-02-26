using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Data;
using Whisprr.Api.Models.Domain;
using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;
using Whisprr.Api.Models.DTOs.SocialTopics;
using Whisprr.Api.Models.DTOs.Subscriptions;

namespace Whisprr.Api.Services;

public class SocialTopicService(AppDbContext dbContext) : ISocialTopicService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<TopicResponse> CreateTopicAsync(Guid userId, CreateTopicRequest request, CancellationToken cancellationToken = default)
    {
        var topic = new SocialTopic
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Keywords = request.Keywords,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SocialTopics.Add(topic);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(topic);
    }

    public async Task<List<TopicSummaryResponse>> GetAllTopicsAsync(CancellationToken cancellationToken = default)
    {
        var topics = await _dbContext.SocialTopics
            .AsNoTracking()
            .Select(t => new TopicSummaryResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                TaskCount = t.Tasks.Count,
                InfoCount = t.SocialInfos.Count
            })
            .ToListAsync(cancellationToken);

        return topics;
    }

    public async Task<int> GetTopicCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SocialTopics.CountAsync(cancellationToken);
    }

    public async Task<TopicDetailResponse?> GetTopicByIdAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        var topic = await _dbContext.SocialTopics
            .AsNoTracking()
            .Include(t => t.Tasks.OrderByDescending(task => task.CreatedAt).Take(5))
            .Include(t => t.SocialInfos.OrderByDescending(info => info.CollectedAt).Take(10))
            .FirstOrDefaultAsync(t => t.Id == topicId, cancellationToken);

        if (topic == null)
            return null;

        return MapToDetailResponse(topic);
    }

    public async Task<TopicResponse?> UpdateTopicAsync(Guid topicId, UpdateTopicRequest request, CancellationToken cancellationToken = default)
    {
        var topic = await _dbContext.SocialTopics.FindAsync(new object[] { topicId }, cancellationToken);

        if (topic == null)
            return null;

        if (request.Name != null)
            topic.Name = request.Name;

        if (request.Description != null)
            topic.Description = request.Description;

        if (request.Keywords != null)
            topic.Keywords = request.Keywords;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(topic);
    }

    public async Task<bool> DeleteTopicAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        var topic = await _dbContext.SocialTopics.FindAsync(new object[] { topicId }, cancellationToken);

        if (topic == null)
            return false;

        _dbContext.SocialTopics.Remove(topic);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<SubscriptionResponse> SubscribeAsync(Guid userId, Guid topicId, SubscribeRequest? request = null, CancellationToken cancellationToken = default)
    {
        // Ensure user exists (minimal data stored locally)
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            // Create minimal user record if not exists
            user = new User
            {
                Id = userId,
                Email = $"user-{userId}@placeholder.com" // Will be updated on first auth
            };
            _dbContext.Users.Add(user);
        }

        // Check if already subscribed
        var existing = await _dbContext.UserTopicSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TopicId == topicId, cancellationToken);

        if (existing != null)
        {
            return MapToSubscriptionResponse(existing);
        }

        var subscription = new UserTopicSubscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TopicId = topicId,
            SubscribedAt = DateTime.UtcNow,
            NotificationPreferences = request?.NotificationPreferences
        };

        _dbContext.UserTopicSubscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Load topic name for response
        await _dbContext.Entry(subscription)
            .Reference(s => s.Topic)
            .LoadAsync(cancellationToken);

        return MapToSubscriptionResponse(subscription);
    }

    public async Task<bool> UnsubscribeAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default)
    {
        var subscription = await _dbContext.UserTopicSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TopicId == topicId, cancellationToken);

        if (subscription == null)
            return false;

        _dbContext.UserTopicSubscriptions.Remove(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<UserSubscriptionsResponse> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscriptions = await _dbContext.UserTopicSubscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.Topic)
            .Select(s => new SubscribedTopicResponse
            {
                SubscriptionId = s.Id,
                TopicId = s.TopicId,
                TopicName = s.Topic.Name,
                SubscribedAt = s.SubscribedAt,
                UnreadCount = 0 // TODO: Implement unread tracking in Stage 2/3
            })
            .ToListAsync(cancellationToken);

        return new UserSubscriptionsResponse
        {
            UserId = userId,
            Subscriptions = subscriptions
        };
    }

    public async Task<bool> IsSubscribedAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserTopicSubscriptions
            .AnyAsync(s => s.UserId == userId && s.TopicId == topicId, cancellationToken);
    }

    private static TopicResponse MapToResponse(SocialTopic topic)
    {
        return new TopicResponse
        {
            Id = topic.Id,
            Name = topic.Name,
            Description = topic.Description,
            Keywords = topic.Keywords,
            CreatedAt = topic.CreatedAt,
            CreatedBy = topic.CreatedBy,
            TaskCount = topic.Tasks?.Count ?? 0,
            InfoCount = topic.SocialInfos?.Count ?? 0,
            SubscriberCount = topic.Subscriptions?.Count ?? 0
        };
    }

    private static TopicDetailResponse MapToDetailResponse(SocialTopic topic)
    {
        return new TopicDetailResponse
        {
            Id = topic.Id,
            Name = topic.Name,
            Description = topic.Description,
            Keywords = topic.Keywords,
            CreatedAt = topic.CreatedAt,
            CreatedBy = topic.CreatedBy,
            TaskCount = topic.Tasks?.Count ?? 0,
            InfoCount = topic.SocialInfos?.Count ?? 0,
            SubscriberCount = topic.Subscriptions?.Count ?? 0,
            RecentTasks = topic.Tasks?.Select(t => new TaskSummaryResponse
            {
                Id = t.Id,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                ItemsCollected = t.ItemsCollected,
                Platform = t.Platform
            }).ToList() ?? [],
            RecentInfo = topic.SocialInfos?.Select(i => new SocialInfoResponse
            {
                Id = i.Id,
                TopicName = topic.Name,
                Platform = i.Platform,
                SourceId = i.SourceId,
                SourceUrl = i.SourceUrl,
                Content = i.Content,
                Author = i.Author,
                PostedAt = i.PostedAt,
                CollectedAt = i.CollectedAt,
                SentimentScore = i.SentimentScore
            }).ToList() ?? []
        };
    }

    private static SubscriptionResponse MapToSubscriptionResponse(UserTopicSubscription subscription)
    {
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            TopicId = subscription.TopicId,
            TopicName = subscription.Topic?.Name ?? "Unknown",
            SubscribedAt = subscription.SubscribedAt
        };
    }
}
