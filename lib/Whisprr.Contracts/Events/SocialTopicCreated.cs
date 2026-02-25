namespace Whisprr.Contracts.Events;

public record SocialTopicCreated
{
  public Guid TopicId { get; set; }
  public Guid CorrelationId { get; init; }
}
