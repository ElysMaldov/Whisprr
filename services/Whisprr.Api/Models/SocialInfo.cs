using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Whisprr.Contracts.Enums;

namespace Whisprr.Api.Models;

/// <summary>
/// Describes a social information from a post, timeline, feed, etc.
/// </summary>
public class SocialInfo
{
  public Guid Id { get; set; }
  public string? Title { get; set; }
  public required string Content { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public Sentiment Sentiment { get; set; }
  [Url]
  public required string OriginalUrl { get; set; }
  public required string OriginalId { get; set; }

  /// <summary>
  /// The platform type this social info came from (e.g., Bluesky, Mastodon).
  /// </summary>
  public required PlatformType Platform { get; set; }

  public Guid GeneratedFromTaskId { get; set; }

  [ForeignKey(nameof(GeneratedFromTaskId))]
  public SocialListeningTask GeneratedFromTask { get; set; } = null!;
}