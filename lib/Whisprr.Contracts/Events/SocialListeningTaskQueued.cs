namespace Whisprr.Contracts.Events;

public class SocialListeningTaskQueued
{
  public Guid TaskId { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
}
