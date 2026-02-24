namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Defines operations for managing and publishing social listening tasks.
/// </summary>
internal interface ISocialListeningTaskPublisher
{
  /// <summary>
  /// Orchestrates the full workflow: arranges tasks from DataSource × SocialTopic combinations,
  /// fetches the queued tasks with populated SocialTopic, and publishes them using the Transactional Outbox pattern.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task ArrangeAndPublishTasks();
}
