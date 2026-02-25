namespace Whisprr.Contracts.Events;

public record UserCreated
{
  public Guid UserId { get; set; }
  public Guid CorrelationId { get; init; }
}
