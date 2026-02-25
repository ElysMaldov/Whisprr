using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Whisprr.Api.Models.Domain;

/// <summary>
/// A topic that users can subscribe to for social listening updates.
/// </summary>
[Index(nameof(CreatedAt))]
public class SocialTopic
{
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Display name of the topic.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }
    
    /// <summary>
    /// Optional description of what this topic tracks.
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Keywords or hashtags to track for this topic.
    /// </summary>
    [Column(TypeName = "text[]")]
    public List<string> Keywords { get; set; } = [];
    
    /// <summary>
    /// When the topic was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of the user who created this topic.
    /// </summary>
    [Required]
    public Guid CreatedBy { get; set; }
    
    // Navigation properties
    public ICollection<SocialListeningTask> Tasks { get; set; } = [];
    public ICollection<SocialInfo> SocialInfos { get; set; } = [];
    public ICollection<UserTopicSubscription> Subscriptions { get; set; } = [];
}
