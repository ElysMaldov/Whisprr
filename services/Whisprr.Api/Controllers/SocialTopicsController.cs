using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisprr.Api.Auth.CurrentUser;
using Whisprr.Api.Models.DTOs.SocialTopics;
using Whisprr.Api.Models.DTOs.Subscriptions;
using Whisprr.Api.Services;

namespace Whisprr.Api.Controllers;

/// <summary>
/// API for managing social listening topics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SocialTopicsController(ISocialTopicService topicService, ICurrentUserProvider currentUser) : ControllerBase
{
    private readonly ISocialTopicService _topicService = topicService;
    private readonly ICurrentUserProvider _currentUser = currentUser;

    /// <summary>
    /// Get all social topics.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TopicSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TopicSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var topics = await _topicService.GetAllTopicsAsync(cancellationToken);
        return Ok(topics);
    }

    /// <summary>
    /// Get total count of topics (public endpoint).
    /// </summary>
    [HttpGet("count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetCount(CancellationToken cancellationToken)
    {
        var count = await _topicService.GetTopicCountAsync(cancellationToken);
        return Ok(count);
    }

    /// <summary>
    /// Get a specific topic by ID with full details.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TopicDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TopicResponse>> Create([FromBody] CreateTopicRequest request, CancellationToken cancellationToken)
    {
        var topic = await _topicService.CreateTopicAsync(_currentUser.UserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
    }

    /// <summary>
    /// Update an existing topic.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TopicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SubscriptionResponse>> Subscribe(Guid id, [FromBody] SubscribeRequest? request = null, CancellationToken cancellationToken = default)
    {
        var subscription = await _topicService.SubscribeAsync(_currentUser.UserId, id, request, cancellationToken);
        return Ok(subscription);
    }

    /// <summary>
    /// Unsubscribe the current user from a topic.
    /// </summary>
    [HttpPost("{id:guid}/unsubscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Unsubscribe(Guid id, CancellationToken cancellationToken)
    {
        var unsubscribed = await _topicService.UnsubscribeAsync(_currentUser.UserId, id, cancellationToken);

        if (!unsubscribed)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Get the current user's subscriptions.
    /// </summary>
    [HttpGet("subscriptions/me")]
    [ProducesResponseType(typeof(UserSubscriptionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserSubscriptionsResponse>> GetMySubscriptions(CancellationToken cancellationToken)
    {
        var subscriptions = await _topicService.GetUserSubscriptionsAsync(_currentUser.UserId, cancellationToken);
        return Ok(subscriptions);
    }

    /// <summary>
    /// Check if the current user is subscribed to a topic.
    /// </summary>
    [HttpGet("{id:guid}/subscription-status")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> GetSubscriptionStatus(Guid id, CancellationToken cancellationToken)
    {
        var isSubscribed = await _topicService.IsSubscribedAsync(_currentUser.UserId, id, cancellationToken);
        return Ok(isSubscribed);
    }
}
