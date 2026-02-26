using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Whisprr.Api.Models.Domain;

/// <summary>
/// A piece of social media content collected by a listening task.
/// </summary>
[Index(nameof(TopicId))]
[Index(nameof(TaskId))]
[Index(nameof(Platform))]
[Index(nameof(CollectedAt))]
[Index(nameof(TopicId), nameof(CollectedAt))]
[Index(nameof(Platform), nameof(SourceId), IsUnique = true)]
public class SocialInfo
{
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The topic this info belongs to.
    /// </summary>
    [Required]
    public Guid TopicId { get; set; }
    
    /// <summary>
    /// The task that collected this info.
    /// </summary>
    [Required]
    public Guid TaskId { get; set; }
    
    /// <summary>
    /// The platform this content came from.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Platform { get; set; }
    
    /// <summary>
    /// Original ID from the source platform.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public required string SourceId { get; set; }
    
    /// <summary>
    /// URL to the original content.
    /// </summary>
    [MaxLength(2000)]
    public string? SourceUrl { get; set; }
    
    /// <summary>
    /// The content text.
    /// </summary>
    [Required]
    public required string Content { get; set; }
    
    /// <summary>
    /// Author/username of the content.
    /// </summary>
    [MaxLength(500)]
    public string? Author { get; set; }
    
    /// <summary>
    /// When the content was posted on the source platform.
    /// </summary>
    public DateTime? PostedAt { get; set; }
    
    /// <summary>
    /// When this info was collected.
    /// </summary>
    [Required]
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Engagement metrics (likes, shares, comments, etc.).
    /// Stored as JSON for flexibility across platforms.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? EngagementData { get; set; }
    
    /// <summary>
    /// Raw response data from the platform API.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? RawData { get; set; }
    
    /// <summary>
    /// Sentiment score if analyzed (-1 to 1).
    /// </summary>
    public double? SentimentScore { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(TopicId))]
    public SocialTopic Topic { get; set; } = null!;
    
    [ForeignKey(nameof(TaskId))]
    public SocialListeningTask Task { get; set; } = null!;
}
