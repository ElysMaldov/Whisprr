using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Whisprr.SocialListeningScheduler.Models;

/// <summary>
/// Describes a platform we use as a source (e.g. Bluesky, Mastodon, etc.)
/// </summary>
[Index(nameof(Name), IsUnique = true)]
internal class DataSource
{
  public Guid Id { get; set; }
  
  [MaxLength(100)]
  public required string Name { get; set; }
  
  [Url]
  [MaxLength(2048)]
  public required string SourceUrl { get; set; }
  
  [Url]
  [MaxLength(2048)]
  public required string IconUrl { get; set; }
  
  public ICollection<SocialListeningTask> SocialListeningTasks { get; set; } = [];
}