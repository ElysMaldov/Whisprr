using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Data;
using Whisprr.Api.Models.Domain;
using Whisprr.Api.Models.DTOs.SocialListeningTasks;

namespace Whisprr.Api.Services;

public class SocialListeningTaskService : ISocialListeningTaskService
{
    private readonly AppDbContext _dbContext;

    public SocialListeningTaskService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskResponse> CreateTaskAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        // Verify topic exists
        var topic = await _dbContext.SocialTopics.FindAsync(new object[] { request.TopicId }, cancellationToken);
        if (topic == null)
            throw new InvalidOperationException($"Topic with ID {request.TopicId} not found");

        var task = new SocialListeningTask
        {
            Id = Guid.NewGuid(),
            TopicId = request.TopicId,
            Platform = request.Platform,
            SearchQuery = request.SearchQuery,
            Status = Models.Domain.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SocialListeningTasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(task, topic.Name);
    }

    public async Task<TaskListResponse> GetAllTasksAsync(TaskFilterRequest? filter = null, CancellationToken cancellationToken = default)
    {
        filter ??= new TaskFilterRequest();
        
        var query = _dbContext.SocialListeningTasks
            .AsNoTracking()
            .Include(t => t.Topic)
            .AsQueryable();

        // Apply filters
        if (filter.TopicId.HasValue)
            query = query.Where(t => t.TopicId == filter.TopicId.Value);
        
        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);
        
        if (!string.IsNullOrEmpty(filter.Platform))
            query = query.Where(t => t.Platform == filter.Platform);

        // Order by most recent
        query = query.OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => MapToResponse(t, t.Topic.Name))
            .ToListAsync(cancellationToken);

        return new TaskListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<TaskResponse?> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.SocialListeningTasks
            .AsNoTracking()
            .Include(t => t.Topic)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task == null)
            return null;

        return MapToResponse(task, task.Topic.Name);
    }

    public async Task<List<TaskSummaryResponse>> GetTasksByTopicAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        var tasks = await _dbContext.SocialListeningTasks
            .AsNoTracking()
            .Where(t => t.TopicId == topicId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskSummaryResponse
            {
                Id = t.Id,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                ItemsCollected = t.ItemsCollected,
                Platform = t.Platform
            })
            .ToListAsync(cancellationToken);

        return tasks;
    }

    public async Task<bool> CancelTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.SocialListeningTasks.FindAsync(new object[] { taskId }, cancellationToken);
        
        if (task == null)
            return false;

        if (task.Status != Models.Domain.TaskStatus.Pending && task.Status != Models.Domain.TaskStatus.Running)
            return false;

        task.Status = Models.Domain.TaskStatus.Cancelled;
        task.CompletedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<TaskResponse?> RetryTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.SocialListeningTasks
            .Include(t => t.Topic)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
        
        if (task == null)
            return null;

        if (task.Status != Models.Domain.TaskStatus.Failed && task.Status != Models.Domain.TaskStatus.Cancelled)
            return null;

        task.Status = Models.Domain.TaskStatus.Pending;
        task.ErrorMessage = null;
        task.CompletedAt = null;
        task.StartedAt = null;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return MapToResponse(task, task.Topic.Name);
    }

    private static TaskResponse MapToResponse(SocialListeningTask task, string topicName)
    {
        return new TaskResponse
        {
            Id = task.Id,
            TopicId = task.TopicId,
            TopicName = topicName,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            StartedAt = task.StartedAt,
            CompletedAt = task.CompletedAt,
            ErrorMessage = task.ErrorMessage,
            ItemsCollected = task.ItemsCollected,
            Platform = task.Platform,
            SearchQuery = task.SearchQuery
        };
    }
}
