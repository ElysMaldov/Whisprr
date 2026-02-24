namespace Whisprr.Contracts.Events;

public record SocialListeningTaskFailed
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; set; }
}
