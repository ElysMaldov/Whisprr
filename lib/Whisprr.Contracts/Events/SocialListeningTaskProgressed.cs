using Whisprr.Contracts.Enums;

namespace Whisprr.Contracts.Events;

public record SocialListeningTaskProgressed
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; init; }
  public DateTimeOffset CreatedAt { get; set; }
  public required PlatformType Platform { get; set; }
  public int FoundSocialInfosCount { get; set; }
}
