using Whisprr.Contracts.Enums;

namespace Whisprr.Contracts.Events;

public record SocialListeningTaskFinished
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; init; }
  public DateTimeOffset FinishedAt { get; set; }
  public TaskProgressStatus Status { get; } = TaskProgressStatus.Success;
  public required string Query;
}
