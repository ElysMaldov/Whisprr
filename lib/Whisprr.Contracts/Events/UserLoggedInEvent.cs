namespace Whisprr.Contracts.Events;

/// <summary>
/// Event published when a user logs in.
/// </summary>
public record UserLoggedInEvent
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public DateTime LoggedInAt { get; init; }
}
