namespace Whisprr.Api.Models.DTOs.Subscriptions;

/// <summary>
/// Response with user's subscribed topics.
/// </summary>
public class UserSubscriptionsResponse
{
    public Guid UserId { get; set; }
    public List<SubscribedTopicResponse> Subscriptions { get; set; } = [];
}
