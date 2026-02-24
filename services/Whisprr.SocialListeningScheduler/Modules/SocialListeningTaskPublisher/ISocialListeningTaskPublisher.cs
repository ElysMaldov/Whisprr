using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

/// <summary>
/// Defines operations for managing and publishing social listening tasks.
/// </summary>
internal interface ISocialListeningTaskPublisher
{
  /// <summary>
  /// Creates new social listening tasks for every DataSource × SocialTopic combination.
  /// Tasks are saved to the database with Pending status.
  /// </summary>
  /// <returns>An array of newly created <see cref="SocialListeningTask"/> with related SocialTopic and DataSource populated.</returns>
  Task<SocialListeningTask[]> CreateNewTasks();

  /// <summary>
  /// Retrieves pending social listening tasks from the database.
  /// </summary>
  /// <returns>An array of pending <see cref="SocialListeningTask"/> with related SocialTopic and DataSource included.</returns>
  Task<SocialListeningTask[]> ArrangeTasks();

  /// <summary>
  /// Publishes the specified social listening tasks.
  /// </summary>
  /// <param name="tasks">The tasks to publish.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task PublishTasks(SocialListeningTask[] tasks);

  /// <summary>
  /// Creates new tasks, retrieves pending tasks, and publishes them.
  /// This is the main orchestration method that combines all operations.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task PublishNewTasks();
}
