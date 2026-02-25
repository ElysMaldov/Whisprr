namespace Whisprr.Api.Models.DTOs.Subscriptions;

/// <summary>
/// Response with subscription details.
/// </summary>
public class SubscriptionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TopicId { get; set; }
    public required string TopicName { get; set; }
    public DateTime SubscribedAt { get; set; }
}
