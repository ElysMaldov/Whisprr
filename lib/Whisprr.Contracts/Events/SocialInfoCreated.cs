using Whisprr.Contracts.Enums;

namespace Whisprr.Contracts.Events;

public record SocialInfoCreated
{
  public Guid InfoId { get; set; }
  public Guid CorrelationId { get; init; }
  public DateTimeOffset CreatedAt { get; set; }
  public string? Title { get; set; }
  public required string Content { get; set; }
  public required string OriginalUrl { get; set; }
  public required string OriginalId { get; set; }
  public required PlatformType Platform { get; set; }
  public required Guid GeneratedFromTaskId { get; set; }
}
