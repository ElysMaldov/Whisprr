using Microsoft.AspNetCore.Mvc;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;
using Whisprr.Api.Services;

namespace Whisprr.Api.Controllers;

/// <summary>
/// API for managing social listening tasks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SocialListeningTasksController : ControllerBase
{
    private readonly ISocialListeningTaskService _taskService;
    private readonly Guid _currentUserId; // TODO: Replace with auth in Stage 3

    public SocialListeningTasksController(ISocialListeningTaskService taskService)
    {
        _taskService = taskService;
        _currentUserId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Hardcoded for Stage 1
    }

    /// <summary>
    /// Get all listening tasks with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TaskListResponse), StatusCodes.Status200OK)]
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
    public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateTaskAsync(_currentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Cancel a running or pending task.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    public async Task<ActionResult<TaskResponse>> Retry(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.RetryTaskAsync(id, cancellationToken);
        
        if (task == null)
            return NotFound();

        return Ok(task);
    }
}
