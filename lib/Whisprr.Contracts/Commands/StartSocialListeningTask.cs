namespace Whisprr.Contracts.Commands;

public record StartSocialListeningTask
{
  public Guid TaskId { get; set; }
  public Guid CorrelationId { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public required string Query { get; set; }
}
