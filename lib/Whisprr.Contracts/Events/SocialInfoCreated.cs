namespace Whisprr.Contracts.Events;

public record SocialInfoCreated
{
  public Guid InfoId { get; set; }
  public Guid CorrelationId { get; init; }

}
