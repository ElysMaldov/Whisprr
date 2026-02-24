using Whisprr.Contracts.Enums;

namespace Whisprr.Contracts.Events;

public record SocialListeningTaskQueued
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public TaskProgressStatus Status { get; } = TaskProgressStatus.Queued;
  public required string Query;
}
