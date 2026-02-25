namespace Whisprr.Api.Models.DTOs.Subscriptions;

/// <summary>
/// Topic info within subscription context.
/// </summary>
public class SubscribedTopicResponse
{
    public Guid SubscriptionId { get; set; }
    public Guid TopicId { get; set; }
    public required string TopicName { get; set; }
    public DateTime SubscribedAt { get; set; }
    public int UnreadCount { get; set; }
}
