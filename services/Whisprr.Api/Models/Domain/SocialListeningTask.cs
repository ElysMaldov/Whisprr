using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Whisprr.Api.Models.Domain;

/// <summary>
/// Status of a social listening task.
/// </summary>
public enum TaskStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// A background task that performs social listening for a specific topic.
/// </summary>
[Index(nameof(TopicId))]
[Index(nameof(Status))]
[Index(nameof(CreatedAt))]
[Index(nameof(TopicId), nameof(Status))]
public class SocialListeningTask
{
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The topic this task is listening for.
    /// </summary>
    [Required]
    public Guid TopicId { get; set; }
    
    /// <summary>
    /// Current status of the task.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    
    /// <summary>
    /// When the task was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the task started execution.
    /// </summary>
    public DateTime? StartedAt { get; set; }
    
    /// <summary>
    /// When the task completed or failed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Error message if the task failed.
    /// </summary>
    [MaxLength(4000)]
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Number of items collected by this task.
    /// </summary>
    public int ItemsCollected { get; set; }
    
    /// <summary>
    /// The platform being monitored (e.g., "Twitter", "Reddit", "Instagram").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Platform { get; set; }
    
    /// <summary>
    /// Optional specific search query for this task.
    /// </summary>
    [MaxLength(1000)]
    public string? SearchQuery { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(TopicId))]
    public SocialTopic Topic { get; set; } = null!;
    public ICollection<SocialInfo> SocialInfos { get; set; } = [];
}
