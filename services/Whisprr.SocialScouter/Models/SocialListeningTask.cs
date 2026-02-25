using Whisprr.Contracts.Enums;
using Whisprr.Utils.Interfaces;

namespace Whisprr.SocialScouter.Models;

internal record SocialListeningTask : ITrackableModel
{
    public required Guid Id { get; set; }
    public required Guid CorrelationId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public required Guid SocialTopicId { get; set; }
    /// <summary>
    /// The platform type for this listening task. Used to route to the correct listener.
    /// </summary>
    public required PlatformType Platform { get; set; }

    public required string Query { get; init; }
}
