using MassTransit;
using Whisprr.Contracts.Enums;
using Whisprr.Contracts.Events;
using Whisprr.SocialListeningScheduler.Data;

namespace Whisprr.SocialListeningScheduler.Modules.MessageBroker.Consumers;

/// <summary>
/// Consumes SocialListeningTaskFinished events and updates the task status to Success.
/// </summary>
internal sealed class SocialListeningTaskFinishedConsumer(
    AppDbContext dbContext,
    ILogger<SocialListeningTaskFinishedConsumer> logger)
    : SocialListeningTaskStatusConsumerBase<SocialListeningTaskFinished>(dbContext, logger)
{
    protected override TaskProgressStatus UpdateToStatus => TaskProgressStatus.Success;

    protected override Guid GetTaskId(SocialListeningTaskFinished message) => message.TaskId;
}
