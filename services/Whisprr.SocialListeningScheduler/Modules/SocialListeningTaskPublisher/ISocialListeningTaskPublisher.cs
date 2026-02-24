using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Defines operations for managing and publishing social listening tasks.
/// </summary>
internal interface ISocialListeningTaskPublisher
{
  /// <summary>
  /// Creates social listening tasks by joining all DataSource × SocialTopic combinations.
  /// Tasks are returned but NOT saved to the database - they will be saved during publishing
  /// using the Transactional Outbox pattern.
  /// </summary>
  /// <returns>An array of <see cref="SocialListeningTask"/> with related SocialTopic and DataSource populated.</returns>
  Task<SocialListeningTask[]> ArrangeTasks();

  /// <summary>
  /// Publishes the specified social listening tasks using the Transactional Outbox pattern.
  /// For each task, saves it to the database and publishes the event atomically.
  /// </summary>
  /// <param name="tasks">The tasks to publish.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task PublishTasks(SocialListeningTask[] tasks);

  /// <summary>
  /// Arranges tasks from DataSource × SocialTopic combinations and publishes them.
  /// This is the main orchestration method that uses the Transactional Outbox pattern.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task ArrangeAndPublishTasks();
}
