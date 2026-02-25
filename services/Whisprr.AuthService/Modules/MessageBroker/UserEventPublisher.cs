using Whisprr.AuthService.Models.Domain;
using Whisprr.Contracts.Events;

namespace Whisprr.AuthService.Modules.MessageBroker;

/// <summary>
/// Publishes user-related domain events to the message broker.
/// </summary>
public class UserEventPublisher : IUserEventPublisher
{
    // TODO: Inject MassTransit IPublishEndpoint when implementing Phase 3
    // private readonly IPublishEndpoint _publishEndpoint;

    public UserEventPublisher()
    {
        // TODO: Accept IPublishEndpoint in constructor when implementing Phase 3
    }

    public Task PublishUserCreatedAsync(User user, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with MassTransit in Phase 3
        // await _publishEndpoint.Publish(new UserCreatedEvent
        // {
        //     UserId = user.Id,
        //     Email = user.Email,
        //     DisplayName = user.DisplayName,
        //     CreatedAt = user.CreatedAt
        // }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task PublishUserLoggedInAsync(User user, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with MassTransit in Phase 3
        // await _publishEndpoint.Publish(new UserLoggedInEvent
        // {
        //     UserId = user.Id,
        //     Email = user.Email,
        //     LoggedInAt = user.LastLoginAt!.Value
        // }, cancellationToken);

        return Task.CompletedTask;
    }
}
