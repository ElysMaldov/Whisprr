# Whisprr.Api Implementation Plan

This document outlines the phased implementation of the API service, designed to minimize risk and allow incremental delivery.

**Core Principle:** Each stage is deployable and testable independently. Business logic remains untouched during auth integration.

---

## Stage 1: Public REST API (Core Functionality)

**Goal:** Build the complete data layer and REST endpoints without auth or real-time features.

**Timeline:** ~3-5 days

### 1.1 Database Schema (EF Core + PostgreSQL)

```
Models/
├── SocialTopic.cs          # Main entity users subscribe to
├── SocialListeningTask.cs  # Background job status
├── SocialInfo.cs           # Crawled social data
└── UserSubscription.cs     # Many-to-many: User <-> Topic
```

**Relationships:**
- `SocialTopic` 1:N `SocialListeningTask`
- `SocialTopic` 1:N `SocialInfo`
- `SocialListeningTask` 1:N `SocialInfo`
- `User` (minimal data) N:M `SocialTopic` (via UserSubscription)

### 1.2 API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/topics` | GET | List all topics (with counts) |
| `/api/topics` | POST | Create new topic (also creates initial task) |
| `/api/topics/{id}` | GET | Get topic details + latest info + tasks |
| `/api/topics/{id}/subscribe` | POST | Subscribe user to topic |
| `/api/topics/{id}/unsubscribe` | POST | Unsubscribe user from topic |
| `/api/tasks` | GET | List all tasks (filterable by status, topic) |
| `/api/tasks/{id}` | GET | Get task status & details |
| `/api/tasks` | POST | Start new listening task |
| `/api/info` | GET | List all social info (filterable by topic, task) |
| `/api/info/{id}` | GET | Get specific social info details |

### 1.3 Project Structure (Stage 1)

```
services/Whisprr.Api/
├── Program.cs
├── appsettings.json
├── Whisprr.Api.csproj
├── Controllers/
│   ├── SocialTopicsController.cs
│   ├── SocialListeningTasksController.cs
│   └── SocialInfoController.cs
├── Models/
│   ├── Entities/
│   │   ├── SocialTopic.cs
│   │   ├── SocialListeningTask.cs
│   │   ├── SocialInfo.cs
│   │   └── UserSubscription.cs
│   └── DTOs/
│       ├── CreateTopicRequest.cs
│       ├── CreateTaskRequest.cs
│       └── (response DTOs)
├── Data/
│   ├── AppDbContext.cs
│   └── Configurations/          # EF entity configurations
├── Services/
│   ├── ISocialTopicService.cs
│   ├── SocialTopicService.cs
│   ├── ISocialListeningTaskService.cs
│   └── SocialListeningTaskService.cs
└── Migrations/
```

### 1.4 Key Implementation Details

**No auth yet** - use a hardcoded `userId` in controllers:

```csharp
// Temporary - replaced in Stage 3
private Guid CurrentUserId => Guid.Parse("11111111-1111-1111-1111-111111111111");
```

**Database queries optimized for the list views:**
- Use `.Include()` and `.Select()` projections
- Add pagination (skip/take) on list endpoints
- Consider DB indexes on: `SocialInfo.TopicId`, `SocialInfo.TaskId`, `UserSubscription.UserId`

### 1.5 Exit Criteria

- [ ] All endpoints return correct data
- [ ] Database schema finalized
- [ ] Swagger docs working
- [ ] Unit tests for services
- [ ] Integration tests for controllers

---

## Stage 2: WebSocket Real-Time Updates (SignalR)

**Goal:** Add live updates when new SocialInfo arrives or Task status changes.

**Timeline:** ~2-3 days

### 2.1 SignalR Hub

```
Hubs/
└── SocialTopicHub.cs
```

**Hub Methods:**

| Method | Direction | Description |
|--------|-----------|-------------|
| `JoinTopic(string topicId)` | Client → Server | Subscribe connection to topic group |
| `LeaveTopic(string topicId)` | Client → Server | Unsubscribe from topic group |
| `OnNewInfo(string topicId, SocialInfoDto info)` | Server → Client | Broadcast new social info |
| `OnTaskStatusChanged(string taskId, string status)` | Server → Client | Broadcast task updates |

### 2.2 Integration Points

**Option A: Direct Hub Injection (Simpler)**
```csharp
// In SocialTopicService
public async Task AddSocialInfoAsync(SocialInfo info)
{
    _dbContext.SocialInfo.Add(info);
    await _dbContext.SaveChangesAsync();
    
    // Notify subscribers
    await _hubContext.Clients.Group(info.TopicId.ToString())
        .SendAsync("OnNewInfo", _mapper.Map<SocialInfoDto>(info));
}
```

**Option B: Event-Based (Better for distributed systems)**
- Publish domain events from services
- Separate notification handler broadcasts to SignalR
- Allows later migration to message queue (RabbitMQ/EventBus)

**Recommendation:** Start with Option A, refactor to Option B in Stage 4 if needed.

### 2.3 Client Connection Flow

```
1. Client calls GET /api/topics/{id} to get initial data
2. Client opens SignalR connection
3. Client calls JoinTopic("topic-id") 
4. Server adds connection to SignalR group
5. Server broadcasts updates to group as they arrive
6. Client calls LeaveTopic on navigation away
```

### 2.4 Scale-Out Consideration

For single instance: Default in-memory SignalR works.

For multiple API instances (future): Add Redis backplane:

```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis("connection-string");
```

### 2.5 Exit Criteria

- [ ] SignalR hub accepts connections
- [ ] Clients receive real-time updates
- [ ] Reconnection logic on client side
- [ ] Load testing (1000 concurrent connections minimum)

---

## Stage 3: Auth Wrapper (Non-Intrusive)

**Goal:** Secure the API without modifying business logic code.

**Timeline:** ~2-3 days

### 3.1 Architecture: The Wrapper Pattern

**Key constraint:** Do NOT change `SocialTopicService`, `SocialListeningTaskService`, etc.

Instead, use ASP.NET Core's built-in extensibility points:

```
Auth/
├── IAuthProxy.cs              # Interface for auth service calls
├── AuthProxy.cs               # HTTP client to Auth Service
├── AuthExtensions.cs          # DI registration
├── CurrentUserProvider.cs     # Scoped service for current user context
├── Authorization/             # Policy handlers
│   ├── TopicAuthorizationHandler.cs
│   └── TopicRequirement.cs
└── Authentication/
    └── JwtValidationConfiguration.cs
```

### 3.2 Implementation Layers (Outside-In)

#### Layer 1: JWT Authentication

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Validate against Auth Service's signing key
        options.Authority = "https://auth-service";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
    });

app.UseAuthentication();
app.UseAuthorization();
```

#### Layer 2: CurrentUserProvider (Scoped Service)

```csharp
public interface ICurrentUserProvider
{
    Guid UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContext;
    
    public CurrentUserProvider(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }
    
    public Guid UserId => Guid.Parse(
        _httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new UnauthorizedException());
    
    public string Email => _httpContext.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? "";
    public bool IsAuthenticated => _httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
```

#### Layer 3: Controller-Level Changes (Minimal)

**Before (Stage 1):**
```csharp
[HttpPost("{id}/subscribe")]
public async Task<IActionResult> Subscribe(Guid id)
{
    var userId = CurrentUserId; // Hardcoded
    await _service.SubscribeUserAsync(id, userId);
    return Ok();
}
```

**After (Stage 3):**
```csharp
[HttpPost("{id}/subscribe")]
[Authorize]  // <-- Only addition
public async Task<IActionResult> Subscribe(Guid id)
{
    var userId = _currentUser.UserId;  // <-- From ICurrentUserProvider
    await _service.SubscribeUserAsync(id, userId);  // Service unchanged!
    return Ok();
}
```

**Changes required per controller:**
1. Add `[Authorize]` attribute
2. Inject `ICurrentUserProvider`
3. Replace hardcoded `CurrentUserId` with `_currentUser.UserId`

**Zero changes to `SocialTopicService`!**

#### Layer 4: Resource-Based Authorization (Optional but Recommended)

```csharp
[HttpGet("{id}")]
[Authorize]
public async Task<IActionResult> GetTopic(Guid id)
{
    var topic = await _service.GetTopicAsync(id);
    
    // Check if user can access this specific topic
    var authorizationResult = await _authorizationService
        .AuthorizeAsync(User, topic, "CanViewTopic");
    
    if (!authorizationResult.Succeeded)
        return Forbid();
    
    return Ok(topic);
}
```

### 3.3 Auth Proxy to Auth Service

For operations requiring Auth Service validation:

```csharp
public class AuthProxy : IAuthProxy
{
    private readonly HttpClient _httpClient;
    
    public AuthProxy(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("AuthService");
    }
    
    public async Task<AuthResult> ValidateTokenAsync(string token)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/validate", new { Token = token });
        // ...
    }
    
    public async Task<UserInfo> GetUserInfoAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}");
        // ...
    }
}
```

### 3.4 WebSocket Auth

SignalR connections also need authentication:

```csharp
// Program.cs
builder.Services.AddSignalR()
    .AddStackExchangeRedis(...) // if using backplane
    ;

// In JavaScript/Dart client:
// connection = new signalR.HubConnectionBuilder()
//     .withUrl("/hubs/social", { accessTokenFactory: () => getJwtToken() })
//     .build();
```

### 3.5 Exit Criteria

- [ ] All endpoints require valid JWT
- [ ] SignalR connections authenticate via token
- [ ] No changes to business service code (verify with `git diff`)
- [ ] Auth proxy communicates with Auth Service
- [ ] Unauthorized requests return 401/403 appropriately

---

## Stage 4: Write-Behind Redis Cache

**Goal:** Optimize read performance without sacrificing data integrity.

**Timeline:** ~3-4 days

### 4.1 Architecture Decision: Cache-Aside vs Write-Behind

**Recommendation:** Start with **Cache-Aside** (simpler, safer), evolve to **Write-Behind** if profiling shows need.

#### Option A: Cache-Aside (Recommended First)

```
Read Path:
  Client → API → Redis? → [Hit: Return] / [Miss: Postgres → Cache → Return]

Write Path:
  Client → API → Postgres → Invalidate Redis
```

#### Option B: Write-Behind (As Specified)

```
Read Path:
  Client → API → Redis → Return (always fast)

Write Path:
  Client → API → Redis (ack immediately)
  Background Worker → Postgres (async persistence)
```

### 4.2 Cache-Aside Implementation (Decorator Pattern)

**Structure:**
```
Services/
├── ISocialTopicService.cs
├── SocialTopicService.cs          # Original (unchanged)
├── ICachingSocialTopicService.cs  # Marker interface
└── CachingSocialTopicService.cs   # Decorator wrapper
```

**Decorator implementation:**

```csharp
public class CachingSocialTopicService : ISocialTopicService
{
    private readonly ISocialTopicService _inner;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingSocialTopicService> _logger;
    
    public CachingSocialTopicService(
        ISocialTopicService inner, 
        IDistributedCache cache,
        ILogger<CachingSocialTopicService> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<TopicDto> GetTopicAsync(Guid id)
    {
        var cacheKey = $"topic:{id}";
        
        // Try cache
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<TopicDto>(cached)!;
        }
        
        // Cache miss - get from DB
        var topic = await _inner.GetTopicAsync(id);
        
        // Store in cache
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(topic),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        
        return topic;
    }
    
    public async Task SubscribeUserAsync(Guid topicId, Guid userId)
    {
        // Write-through: DB first
        await _inner.SubscribeUserAsync(topicId, userId);
        
        // Invalidate affected caches
        await _cache.RemoveAsync($"topic:{topicId}");
        await _cache.RemoveAsync($"user:{userId}:subscriptions");
    }
}
```

**DI Registration:**
```csharp
// Program.cs
builder.Services.AddScoped<SocialTopicService>();  // Concrete
builder.Services.AddScoped<ISocialTopicService>(provider => 
{
    var inner = provider.GetRequiredService<SocialTopicService>();
    var cache = provider.GetRequiredService<IDistributedCache>();
    var logger = provider.GetRequiredService<ILogger<CachingSocialTopicService>>();
    return new CachingSocialTopicService(inner, cache, logger);
});
```

### 4.3 Write-Behind Implementation (If Needed Later)

**Components:**
```
BackgroundServices/
├── WriteBehindSyncService.cs      # Background worker
├── IWriteBehindQueue.cs           # Abstraction
└── RedisWriteBehindQueue.cs       # Redis-based queue

Models/
└── WriteBehindOperation.cs        # Enqueue operations
```

**Flow:**
```csharp
// API writes to Redis
public async Task AddSocialInfoAsync(SocialInfo info)
{
    // 1. Save to Redis immediately
    await _redis.HashSetAsync($"info:{info.Id}", info.ToHashEntries());
    await _redis.ListRightPushAsync("write-behind:queue", info.Id.ToString());
    
    // 2. Notify subscribers (Stage 2)
    await _hubContext.Clients.Group(info.TopicId.ToString())
        .SendAsync("OnNewInfo", info);
}

// Background worker flushes to Postgres
public class WriteBehindSyncService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var batch = await _redis.ListRangeAsync("write-behind:queue", 0, 99);
            if (batch.Length == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }
            
            // Batch insert to Postgres
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var infos = await LoadFromRedisAsync(batch);
                await _dbContext.SocialInfo.AddRangeAsync(infos);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                
                // Remove from queue
                await _redis.ListTrimAsync("write-behind:queue", batch.Length, -1);
            }
            catch
            {
                await transaction.RollbackAsync();
                // Log, alert, retry logic
            }
        }
    }
}
```

**Risk mitigation for Write-Behind:**
- Redis persistence enabled (AOF + RDB)
- Dead letter queue for failed writes
- Monitoring/alerting on queue depth
- Idempotency keys to prevent duplicates

### 4.4 Cache Invalidation Strategy

**Time-based:**
- Hot data (active topics): 5 minutes
- Warm data (topic lists): 15 minutes
- Cold data (old social info): 1 hour

**Event-based:**
- Subscribe/Unsubscribe: Invalidate topic + user subscription cache
- New SocialInfo: Add to cache (don't invalidate)
- Task status change: Update cache entry

### 4.5 Exit Criteria

- [ ] Redis cache configured and connected
- [ ] Read operations hit cache (verified via logs)
- [ ] Write operations invalidate stale cache
- [ ] Cache hit rate > 80% (monitoring)
- [ ] Background sync queue stays near 0 (for write-behind)
- [ ] Data consistency verified (compare Redis vs Postgres)

---

## Cross-Cutting Concerns

### Logging & Monitoring

Add at each stage:

```csharp
// Structured logging for all operations
_logger.LogInformation("User {UserId} subscribed to topic {TopicId}", userId, topicId);

// Metrics
// - Request duration histogram
// - Cache hit/miss ratio
// - SignalR connection count
// - Background queue depth
```

### Error Handling

```
Middleware/
└── GlobalExceptionMiddleware.cs

Exceptions/
├── NotFoundException.cs
├── UnauthorizedException.cs
└── ValidationException.cs
```

Consistent error response format:
```json
{
  "error": "TopicNotFound",
  "message": "Topic with id '123' was not found",
  "traceId": "00-abc123"
}
```

### Testing Strategy

| Stage | Unit Tests | Integration Tests | E2E Tests |
|-------|-----------|-------------------|-----------|
| 1 | Services, EF configs | Controllers + InMemory DB | API contract |
| 2 | Hub methods | SignalR connection | Real-time updates |
| 3 | Auth policies | JWT validation | Full auth flow |
| 4 | Cache decorator | Redis container | Performance |

---

## Deployment Checklist

### Stage 1
- [ ] PostgreSQL instance running
- [ ] Migrations applied
- [ ] API deployed with Swagger exposed

### Stage 2
- [ ] SignalR endpoint accessible (WebSocket support)
- [ ] Client SDK updated

### Stage 3
- [ ] Auth Service reachable
- [ ] JWT signing key synced
- [ ] CORS configured for client domains

### Stage 4
- [ ] Redis instance running
- [ ] Persistence configured (AOF enabled)
- [ ] Memory limits set
- [ ] Eviction policy configured (allkeys-lru)

---

## Migration Path Summary

```
Stage 1 ──────┐
              ├──► Stage 2 ──────┐
              │    (SignalR)     │
              │                  ├──► Stage 3 ──────┐
              │                  │    (Auth)        │
              │                  │                  ├──► Stage 4
              │                  │                  │    (Redis)
Postgres ◄────┘                  │                  │
                                 │                  │
                                 └─────► SignalR ◄──┤
                                                      │
                                                      └─────► Redis ◄──► Postgres
```

Each arrow represents a non-breaking addition. You can deploy and test each stage independently.
