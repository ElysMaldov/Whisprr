using System.ComponentModel.DataAnnotations;

namespace Whisprr.Api.Models;

/// <summary>
/// Describes a platform we use as a source (e.g. Bluesky, Mastodon, etc.)
/// </summary>
public class DataSource
{
  public Guid Id { get; set; }
  [MaxLength(100)]
  public required string Name { get; set; }
  [Url]
  public required string SourceUrl { get; set; }
  [Url]
  public required string IconUrl { get; set; }
  /// <summary>
  /// Collection navigation
  /// </summary>
  public ICollection<SocialInfo> SocialInfos { get; set; } = [];
  public ICollection<SocialListeningTask> SocialListeningTasks { get; set; } = [];
}