
using Whisprr.Utils.Interfaces;

namespace Whisprr.SocialScouter.Models;

internal record SocialListeningTask : ITrackableModel
{
  public required Guid Id { get; set; }
  public required DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }

  public required Guid SocialTopicId { get; set; }
  public required Guid SourcePlatformId { get; set; }
  public required string Query { get; init; }
}