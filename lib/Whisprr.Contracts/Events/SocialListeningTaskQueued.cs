namespace Whisprr.Contracts.Events;

public record SocialListeningTaskQueued
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
}
