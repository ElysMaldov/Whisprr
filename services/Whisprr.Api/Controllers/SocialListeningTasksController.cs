using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisprr.Api.Auth.CurrentUser;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;
using Whisprr.Api.Services;

namespace Whisprr.Api.Controllers;

/// <summary>
/// API for managing social listening tasks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SocialListeningTasksController(ISocialListeningTaskService taskService, ICurrentUserProvider currentUser) : ControllerBase
{
    private readonly ISocialListeningTaskService _taskService = taskService;
    private readonly ICurrentUserProvider _currentUser = currentUser;

    /// <summary>
    /// Get all listening tasks with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TaskListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskListResponse>> GetAll([FromQuery] TaskFilterRequest? filter, CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetAllTasksAsync(filter, cancellationToken);
        return Ok(tasks);
    }

    /// <summary>
    /// Get a specific task by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    /// <summary>
    /// Get all tasks for a specific topic.
    /// </summary>
    [HttpGet("by-topic/{topicId:guid}")]
    [ProducesResponseType(typeof(List<TaskSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TaskSummaryResponse>>> GetByTopic(Guid topicId, CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetTasksByTopicAsync(topicId, cancellationToken);
        return Ok(tasks);
    }

    /// <summary>
    /// Create and start a new listening task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateTaskAsync(_currentUser.UserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Cancel a running or pending task.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var cancelled = await _taskService.CancelTaskAsync(id, cancellationToken);

        if (!cancelled)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Retry a failed or cancelled task.
    /// </summary>
    [HttpPost("{id:guid}/retry")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> Retry(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.RetryTaskAsync(id, cancellationToken);

        if (task == null)
            return NotFound();

        return Ok(task);
    }
}
