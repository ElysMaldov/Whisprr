using Whisprr.Contracts.Enums;
using Whisprr.Utils.Interfaces;

namespace Whisprr.Api.Models;

public class SocialListeningTask : ITrackableModel
{
  public Guid Id { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }

  public TaskProgressStatus Status { get; set; }

  public Guid SocialTopicId { get; set; }
  public SocialTopic SocialTopic { get; set; } = null!; // Use dammit to avoid this field being nullable by compiler, but will be populated by EF Core. Kind of like late in dart.

  /// <summary>
  /// The platform type for this listening task (e.g., Bluesky, Mastodon).
  /// </summary>
  public required PlatformType Platform { get; set; }
  public ICollection<SocialInfo> GeneratedSocialInfos { get; set; } = []; // Since the name doesn't match convention, we setup the relationship using FLuent API in the context

  public string Query
  {
    get => string.Join(" ", SocialTopic.Keywords);
  }
}