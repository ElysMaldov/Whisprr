namespace Whisprr.Contracts.Events;

public record SocialListeningTaskFinished
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; set; }
}
