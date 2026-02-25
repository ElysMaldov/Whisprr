namespace Whisprr.Api.Models.DTOs.Subscriptions;

/// <summary>
/// Request to subscribe to a topic.
/// </summary>
public class SubscribeRequest
{
    /// <summary>
    /// Optional notification preferences JSON.
    /// </summary>
    public string? NotificationPreferences { get; set; }
}
