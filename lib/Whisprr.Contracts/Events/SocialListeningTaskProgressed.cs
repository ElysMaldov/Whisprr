namespace Whisprr.Contracts.Events;

public record SocialListeningTaskProgressed
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; set; }
}
