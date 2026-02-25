using Microsoft.AspNetCore.Mvc;
using Whisprr.Api.Models.DTOs.SocialTopics;
using Whisprr.Api.Models.DTOs.Subscriptions;
using Whisprr.Api.Services;

namespace Whisprr.Api.Controllers;

/// <summary>
/// API for managing social listening topics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SocialTopicsController : ControllerBase
{
    private readonly ISocialTopicService _topicService;
    private readonly Guid _currentUserId; // TODO: Replace with auth in Stage 3

    public SocialTopicsController(ISocialTopicService topicService)
    {
        _topicService = topicService;
        _currentUserId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Hardcoded for Stage 1
    }

    /// <summary>
    /// Get all social topics.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TopicSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TopicSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var topics = await _topicService.GetAllTopicsAsync(cancellationToken);
        return Ok(topics);
    }

    /// <summary>
    /// Get a specific topic by ID with full details.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TopicDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TopicDetailResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var topic = await _topicService.GetTopicByIdAsync(id, cancellationToken);
        
        if (topic == null)
            return NotFound();

        return Ok(topic);
    }

    /// <summary>
    /// Create a new social topic.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TopicResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TopicResponse>> Create([FromBody] CreateTopicRequest request, CancellationToken cancellationToken)
    {
        var topic = await _topicService.CreateTopicAsync(_currentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
    }

    /// <summary>
    /// Update an existing topic.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TopicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TopicResponse>> Update(Guid id, [FromBody] UpdateTopicRequest request, CancellationToken cancellationToken)
    {
        var topic = await _topicService.UpdateTopicAsync(id, request, cancellationToken);
        
        if (topic == null)
            return NotFound();

        return Ok(topic);
    }

    /// <summary>
    /// Delete a topic.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _topicService.DeleteTopicAsync(id, cancellationToken);
        
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Subscribe the current user to a topic.
    /// </summary>
    [HttpPost("{id:guid}/subscribe")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionResponse>> Subscribe(Guid id, [FromBody] SubscribeRequest? request = null, CancellationToken cancellationToken = default)
    {
        var subscription = await _topicService.SubscribeAsync(_currentUserId, id, request, cancellationToken);
        return Ok(subscription);
    }

    /// <summary>
    /// Unsubscribe the current user from a topic.
    /// </summary>
    [HttpPost("{id:guid}/unsubscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unsubscribe(Guid id, CancellationToken cancellationToken)
    {
        var unsubscribed = await _topicService.UnsubscribeAsync(_currentUserId, id, cancellationToken);
        
        if (!unsubscribed)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Get the current user's subscriptions.
    /// </summary>
    [HttpGet("subscriptions/me")]
    [ProducesResponseType(typeof(UserSubscriptionsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSubscriptionsResponse>> GetMySubscriptions(CancellationToken cancellationToken)
    {
        var subscriptions = await _topicService.GetUserSubscriptionsAsync(_currentUserId, cancellationToken);
        return Ok(subscriptions);
    }

    /// <summary>
    /// Check if the current user is subscribed to a topic.
    /// </summary>
    [HttpGet("{id:guid}/subscription-status")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> GetSubscriptionStatus(Guid id, CancellationToken cancellationToken)
    {
        var isSubscribed = await _topicService.IsSubscribedAsync(_currentUserId, id, cancellationToken);
        return Ok(isSubscribed);
    }
}
