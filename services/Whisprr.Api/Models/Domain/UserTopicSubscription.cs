using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Whisprr.Api.Models.Domain;

/// <summary>
/// Many-to-many relationship between users and topics they subscribe to.
/// </summary>
[Index(nameof(UserId))]
[Index(nameof(TopicId))]
[Index(nameof(UserId), nameof(TopicId), IsUnique = true)]
public class UserTopicSubscription
{
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// ID of the subscribing user.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// ID of the topic being subscribed to.
    /// </summary>
    [Required]
    public Guid TopicId { get; set; }
    
    /// <summary>
    /// When the subscription was created.
    /// </summary>
    [Required]
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Notification preferences (JSON for flexibility).
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? NotificationPreferences { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    
    [ForeignKey(nameof(TopicId))]
    public SocialTopic Topic { get; set; } = null!;
}
