using Whisprr.AuthService.Models.Domain;

namespace Whisprr.AuthService.Modules.MessageBroker;

/// <summary>
/// Publishes user-related domain events.
/// </summary>
public interface IUserEventPublisher
{
    /// <summary>
    /// Publishes an event when a new user is created.
    /// </summary>
    Task PublishUserCreatedAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an event when a user logs in.
    /// </summary>
    Task PublishUserLoggedInAsync(User user, CancellationToken cancellationToken = default);
}
