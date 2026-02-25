using MassTransit;
using Whisprr.Contracts.Enums;
using Whisprr.Contracts.Events;
using Whisprr.SocialListeningScheduler.Data;

namespace Whisprr.SocialListeningScheduler.Modules.MessageBroker.Consumers;

/// <summary>
/// Consumes SocialListeningTaskFailed events and updates the task status to Failed.
/// </summary>
internal sealed class SocialListeningTaskFailedConsumer(
    AppDbContext dbContext,
    ILogger<SocialListeningTaskFailedConsumer> logger)
    : SocialListeningTaskStatusConsumerBase<SocialListeningTaskFailed>(dbContext, logger)
{
    protected override TaskProgressStatus UpdateToStatus => TaskProgressStatus.Failed;

    protected override Guid GetTaskId(SocialListeningTaskFailed message) => message.TaskId;
}
