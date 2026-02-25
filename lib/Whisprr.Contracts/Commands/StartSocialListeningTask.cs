namespace Whisprr.Contracts.Commands;

public record StartSocialListeningTask
{
  public required Guid TaskId { get; set; }
  public Guid CorrelationId { get; init; }

  public required Guid SocialTopicId { get; set; }
  public required Guid SourcePlatformId { get; set; }
  public required DateTimeOffset CreatedAt { get; set; }
  public required string Query { get; set; }
}
