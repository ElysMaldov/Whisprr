using Whisprr.Contracts.Enums;
using Whisprr.Utils.Interfaces;

namespace Whisprr.SocialListeningScheduler.Models;

internal class SocialListeningTask : ITrackableModel
{
  public Guid Id { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }

  public TaskProgressStatus Status { get; set; }

  public Guid SocialTopicId { get; set; }
  public SocialTopic SocialTopic { get; set; } = null!;

  /// <summary>
  /// The platform type for this listening task (e.g., Bluesky, Mastodon).
  /// </summary>
  public PlatformType Platform { get; set; }

  /// <summary>
  /// Computed query from the SocialTopic keywords.
  /// </summary>
  public string Query => string.Join(" ", SocialTopic.Keywords);
}
