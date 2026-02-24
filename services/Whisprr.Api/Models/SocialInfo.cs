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
  [MaxLength(100)]
  public string? Title { get; set; }
  [MaxLength(1_000)]
  public required string Content { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public Sentiment Sentiment { get; set; }
  [Url]
  public required string OriginalUrl { get; set; }
  public required string OriginalId { get; set; }
  /// <summary>
  /// PK for <see cref="SourcePlatform"/>
  /// </summary>
  public Guid SourcePlatformId { get; set; }
  /// <summary>
  /// Reference navigation to populate the data when fetched
  /// </summary>
  public DataSource DataSource { get; set; } = null!;

  public Guid GeneratedFromTaskId { get; set; }

  [ForeignKey(nameof(GeneratedFromTaskId))]
  public SocialListeningTask GeneratedFromTask { get; set; } = null!;
}