using MassTransit;
using Whisprr.Contracts.Enums;
using Whisprr.Contracts.Events;
using Whisprr.SocialListeningScheduler.Data;

namespace Whisprr.SocialListeningScheduler.Modules.MessageBroker.Consumers;

/// <summary>
/// Consumes SocialListeningTaskQueued events and updates the task status to Queued.
/// Note: Tasks are already created with Queued status, this confirms receipt by SocialScouter.
/// </summary>
internal sealed class SocialListeningTaskQueuedConsumer(
    AppDbContext dbContext,
    ILogger<SocialListeningTaskQueuedConsumer> logger)
    : SocialListeningTaskStatusConsumerBase<SocialListeningTaskQueued>(dbContext, logger)
{
    protected override TaskProgressStatus UpdateToStatus => TaskProgressStatus.Queued;

    protected override Guid GetTaskId(SocialListeningTaskQueued message) => message.TaskId;
}
