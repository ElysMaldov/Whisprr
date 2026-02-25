using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Data;
using Whisprr.Api.Models.DTOs.SocialInfo;

namespace Whisprr.Api.Services;

public class SocialInfoService : ISocialInfoService
{
    private readonly AppDbContext _dbContext;

    public SocialInfoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SocialInfoListResponse> GetAllAsync(SocialInfoFilterRequest? filter = null, CancellationToken cancellationToken = default)
    {
        filter ??= new SocialInfoFilterRequest();
        
        var query = _dbContext.SocialInfos
            .AsNoTracking()
            .Include(i => i.Topic)
            .AsQueryable();

        // Apply filters
        if (filter.TopicId.HasValue)
            query = query.Where(i => i.TopicId == filter.TopicId.Value);
        
        if (filter.TaskId.HasValue)
            query = query.Where(i => i.TaskId == filter.TaskId.Value);
        
        if (!string.IsNullOrEmpty(filter.Platform))
            query = query.Where(i => i.Platform == filter.Platform);
        
        if (filter.FromDate.HasValue)
            query = query.Where(i => i.CollectedAt >= filter.FromDate.Value);
        
        if (filter.ToDate.HasValue)
            query = query.Where(i => i.CollectedAt <= filter.ToDate.Value);

        // Order by most recent
        query = query.OrderByDescending(i => i.CollectedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => MapToResponse(i, i.Topic.Name))
            .ToListAsync(cancellationToken);

        return new SocialInfoListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<SocialInfoResponse?> GetByIdAsync(Guid infoId, CancellationToken cancellationToken = default)
    {
        var info = await _dbContext.SocialInfos
            .AsNoTracking()
            .Include(i => i.Topic)
            .FirstOrDefaultAsync(i => i.Id == infoId, cancellationToken);

        if (info == null)
            return null;

        return MapToResponse(info, info.Topic.Name);
    }

    public async Task<SocialInfoListResponse> GetByTopicAsync(Guid topicId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SocialInfos
            .AsNoTracking()
            .Where(i => i.TopicId == topicId)
            .Include(i => i.Topic)
            .OrderByDescending(i => i.CollectedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => MapToResponse(i, i.Topic.Name))
            .ToListAsync(cancellationToken);

        return new SocialInfoListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<SocialInfoListResponse> GetByTaskAsync(Guid taskId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SocialInfos
            .AsNoTracking()
            .Where(i => i.TaskId == taskId)
            .Include(i => i.Topic)
            .OrderByDescending(i => i.CollectedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => MapToResponse(i, i.Topic.Name))
            .ToListAsync(cancellationToken);

        return new SocialInfoListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<SocialInfoResponse>> GetRecentByTopicAsync(Guid topicId, int count = 20, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.SocialInfos
            .AsNoTracking()
            .Where(i => i.TopicId == topicId)
            .Include(i => i.Topic)
            .OrderByDescending(i => i.CollectedAt)
            .Take(count)
            .Select(i => MapToResponse(i, i.Topic.Name))
            .ToListAsync(cancellationToken);

        return items;
    }

    private static SocialInfoResponse MapToResponse(Models.Domain.SocialInfo info, string topicName)
    {
        return new SocialInfoResponse
        {
            Id = info.Id,
            TopicId = info.TopicId,
            TopicName = topicName,
            TaskId = info.TaskId,
            Platform = info.Platform,
            SourceId = info.SourceId,
            SourceUrl = info.SourceUrl,
            Content = info.Content,
            Author = info.Author,
            PostedAt = info.PostedAt,
            CollectedAt = info.CollectedAt,
            SentimentScore = info.SentimentScore
        };
    }
}
