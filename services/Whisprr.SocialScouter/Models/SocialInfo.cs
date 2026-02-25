using System.ComponentModel.DataAnnotations;
using Whisprr.Contracts.Enums;

namespace Whisprr.SocialScouter.Models;

/// <summary>
/// Describes a social information from a post, timeline, feed, etc.
/// </summary>
internal record SocialInfo
{
  public Guid Id { get; set; }
  [MaxLength(100)]
  public string? Title { get; set; }
  [MaxLength(1_000)]
  public required string Content { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public Sentiment Sentiment { get; set; }
  [Url]
  public required string OriginalUrl { get; set; }
  public required string OriginalId { get; set; }
  public required Guid SourcePlatformId { get; set; }

  /// <summary>
  /// The platform type this social info came from (e.g., Bluesky, Mastodon).
  /// </summary>
  public required PlatformType Platform { get; set; }

  public Guid GeneratedFromTaskId { get; set; }
}