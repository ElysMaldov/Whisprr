namespace Whisprr.Contracts.Events;

/// <summary>
/// Event published when a new user is created.
/// </summary>
public record UserCreatedEvent
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
    public DateTime CreatedAt { get; init; }
}
