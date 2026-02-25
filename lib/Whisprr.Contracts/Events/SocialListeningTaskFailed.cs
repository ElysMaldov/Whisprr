using Whisprr.Contracts.Enums;

namespace Whisprr.Contracts.Events;

public record SocialListeningTaskFailed
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; init; }
  public DateTimeOffset FailedAt { get; set; }
  public TaskProgressStatus Status { get; } = TaskProgressStatus.Failed;
  public required string Query;
}
