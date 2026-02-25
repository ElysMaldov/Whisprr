using Microsoft.AspNetCore.Mvc;
using Whisprr.Api.Models.DTOs.SocialInfo;
using Whisprr.Api.Services;

namespace Whisprr.Api.Controllers;

/// <summary>
/// API for accessing collected social information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SocialInfoController : ControllerBase
{
    private readonly ISocialInfoService _infoService;

    public SocialInfoController(ISocialInfoService infoService)
    {
        _infoService = infoService;
    }

    /// <summary>
    /// Get all social info with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SocialInfoListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SocialInfoListResponse>> GetAll([FromQuery] SocialInfoFilterRequest? filter, CancellationToken cancellationToken)
    {
        var info = await _infoService.GetAllAsync(filter, cancellationToken);
        return Ok(info);
    }

    /// <summary>
    /// Get a specific social info entry by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SocialInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SocialInfoResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var info = await _infoService.GetByIdAsync(id, cancellationToken);
        
        if (info == null)
            return NotFound();

        return Ok(info);
    }

    /// <summary>
    /// Get all social info for a specific topic.
    /// </summary>
    [HttpGet("by-topic/{topicId:guid}")]
    [ProducesResponseType(typeof(SocialInfoListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SocialInfoListResponse>> GetByTopic(
        Guid topicId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50, 
        CancellationToken cancellationToken = default)
    {
        var info = await _infoService.GetByTopicAsync(topicId, page, pageSize, cancellationToken);
        return Ok(info);
    }

    /// <summary>
    /// Get all social info for a specific task.
    /// </summary>
    [HttpGet("by-task/{taskId:guid}")]
    [ProducesResponseType(typeof(SocialInfoListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SocialInfoListResponse>> GetByTask(
        Guid taskId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50, 
        CancellationToken cancellationToken = default)
    {
        var info = await _infoService.GetByTaskAsync(taskId, page, pageSize, cancellationToken);
        return Ok(info);
    }

    /// <summary>
    /// Get recent social info for a topic (for live updates).
    /// </summary>
    [HttpGet("by-topic/{topicId:guid}/recent")]
    [ProducesResponseType(typeof(List<SocialInfoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SocialInfoResponse>>> GetRecentByTopic(
        Guid topicId, 
        [FromQuery] int count = 20, 
        CancellationToken cancellationToken = default)
    {
        var info = await _infoService.GetRecentByTopicAsync(topicId, count, cancellationToken);
        return Ok(info);
    }
}
