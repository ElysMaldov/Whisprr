using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Whisprr.Api.Models.Domain;

/// <summary>
/// Minimal user data stored locally. Full authentication is handled by Auth Service.
/// </summary>
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(ExternalAuthId), IsUnique = true)]
[Index(nameof(IsActive))]
public class User
{
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// User's email address (for notifications and identification).
    /// </summary>
    [Required]
    [MaxLength(256)]
    public required string Email { get; set; }
    
    /// <summary>
    /// Display name chosen by the user.
    /// </summary>
    [MaxLength(200)]
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// External ID from the Auth Service.
    /// </summary>
    [MaxLength(500)]
    public string? ExternalAuthId { get; set; }
    
    /// <summary>
    /// When the user was first seen by this API.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the user data was last updated.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Whether this user account is active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<UserTopicSubscription> Subscriptions { get; set; } = [];
}
