using MassTransit;
using Whisprr.AuthService.Models.Domain;
using Whisprr.Contracts.Events;

namespace Whisprr.AuthService.Modules.MessageBroker;

/// <summary>
/// Publishes user-related domain events to the message broker.
/// </summary>
public class UserEventPublisher(IPublishEndpoint publishEndpoint) : IUserEventPublisher
{
    public async Task PublishUserCreatedAsync(User user, CancellationToken cancellationToken = default)
    {
        await publishEndpoint.Publish(new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt
        }, cancellationToken);
    }

    public async Task PublishUserLoggedInAsync(User user, CancellationToken cancellationToken = default)
    {
        await publishEndpoint.Publish(new UserLoggedInEvent
        {
            UserId = user.Id,
            Email = user.Email,
            LoggedInAt = user.LastLoginAt!.Value
        }, cancellationToken);
    }
}
