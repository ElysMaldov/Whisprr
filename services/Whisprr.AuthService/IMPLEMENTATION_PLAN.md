# AuthService Implementation Plan

This document outlines the complete implementation of the AuthService microservice, which handles user authentication, registration, and emits domain events for the Whisprr ecosystem.

---

## Overview

**Purpose:** Centralized authentication service for all Whisprr services.

**Responsibilities:**
- User registration and login
- JWT token generation and validation
- User profile management
- Publish auth-related domain events

**Integration:**
- Consumed by Whisprr.Api (via AuthProxy)
- Publishes events to MessageBroker (for audit, analytics, etc.)

---

## Architecture

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│   Client App    │────▶│  Whisprr.Api     │────▶│  AuthService    │
│  (Web/Mobile)   │     │  (Proxies Auth)  │     │  (This Service) │
└─────────────────┘     └──────────────────┘     └─────────────────┘
                              │                           │
                              │                           │
                              ▼                           ▼
                       ┌──────────────┐          ┌─────────────────┐
                       │  JWT Token   │          │  PostgreSQL     │
                       │  (Returned   │          │  (User Store)   │
                       │   to Client) │          └─────────────────┘
                       └──────────────┘                   │
                                                          │
                                                          ▼
                                                   ┌─────────────────┐
                                                   │  MessageBroker  │
                                                   │  (Events)       │
                                                   └─────────────────┘
```

---

## Project Structure

```
services/Whisprr.AuthService/
├── Whisprr.AuthService.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── IMPLEMENTATION_PLAN.md
│
├── Data/
│   ├── AuthDbContext.cs
│   └── Configurations/
│       └── UserConfiguration.cs
│
├── Migrations/
│   └── (EF Core migrations)
│
├── Models/
│   ├── Domain/
│   │   └── User.cs
│   └── DTOs/
│       ├── RegisterRequest.cs
│       ├── LoginRequest.cs
│       ├── AuthResponse.cs
│       ├── UserResponse.cs
│       └── ValidateTokenRequest.cs
│
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── ITokenService.cs
│   └── TokenService.cs
│
├── Controllers/
│   └── AuthController.cs
│
├── MessageBroker/
│   ├── IUserEventPublisher.cs
│   └── UserEventPublisher.cs
│
└── Infrastructure/
    └── AuthExtensions.cs
```

---

## Phase 1: Project Setup & Database

### 1.1 Create Project

```bash
dotnet new web -n Whisprr.AuthService -o services/Whisprr.AuthService
cd services/Whisprr.AuthService
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package MassTransit
dotnet add package MassTransit.RabbitMQ  # or MassTransit.Azure.ServiceBus.Core
dotnet add package Swashbuckle.AspNetCore
```

### 1.2 User Domain Model

```csharp
// Models/Domain/User.cs
public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}
```

### 1.3 Database Context

```csharp
// Data/AuthDbContext.cs
public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }
}

// Data/Configurations/UserConfiguration.cs
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(200);
    }
}
```

### 1.4 Configuration

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=whisprr_auth;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-characters-long",
    "Issuer": "whisprr-auth",
    "Audience": "whisprr-api",
    "ExpiryHours": 24
  },
  "MessageBroker": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

---

## Phase 2: Authentication Services

### 2.1 Token Service

```csharp
// Services/ITokenService.cs
public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}

// Services/TokenService.cs
public class TokenService(IConfiguration config) : ITokenService
{
    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName ?? user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(double.Parse(config["Jwt:ExpiryHours"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid? ValidateToken(string token)
    {
        // Implementation for token validation
    }
    
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
```

### 2.2 Auth Service

```csharp
// Services/IAuthService.cs
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResult> ValidateTokenAsync(string token);
    Task<UserResponse?> GetUserAsync(Guid userId);
}

// Services/AuthService.cs
public partial class AuthService(
    AuthDbContext dbContext,
    ITokenService tokenService,
    IUserEventPublisher eventPublisher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        if (await dbContext.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already registered");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            DisplayName = request.DisplayName
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Generate tokens
        var token = tokenService.GenerateJwtToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Publish event
        await eventPublisher.PublishUserCreatedAsync(user);

        LogUserRegistered(logger, user.Id, user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        
        // Generate tokens
        var token = tokenService.GenerateJwtToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        await dbContext.SaveChangesAsync();

        // Publish event
        await eventPublisher.PublishUserLoggedInAsync(user);

        LogUserLoggedIn(logger, user.Id, user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "User registered: {UserId} ({Email})")]
    static partial void LogUserRegistered(ILogger<AuthService> logger, Guid userId, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "User logged in: {UserId} ({Email})")]
    static partial void LogUserLoggedIn(ILogger<AuthService> logger, Guid userId, string email);
}
```

---

## Phase 3: Message Broker Integration

### 3.1 Event Contracts (Shared)

```csharp
// In Whisprr.Contracts project
public record UserCreatedEvent
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record UserLoggedInEvent
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public DateTime LoggedInAt { get; init; }
}
```

### 3.2 Event Publisher

```csharp
// MessageBroker/IUserEventPublisher.cs
public interface IUserEventPublisher
{
    Task PublishUserCreatedAsync(User user);
    Task PublishUserLoggedInAsync(User user);
}

// MessageBroker/UserEventPublisher.cs
public class UserEventPublisher(IPublishEndpoint publishEndpoint) : IUserEventPublisher
{
    public async Task PublishUserCreatedAsync(User user)
    {
        await publishEndpoint.Publish(new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt
        });
    }

    public async Task PublishUserLoggedInAsync(User user)
    {
        await publishEndpoint.Publish(new UserLoggedInEvent
        {
            UserId = user.Id,
            Email = user.Email,
            LoggedInAt = user.LastLoginAt!.Value
        });
    }
}
```

### 3.3 MassTransit Configuration

```csharp
// Infrastructure/MessageBrokerExtensions.cs
public static class MessageBrokerExtensions
{
    public static IHostApplicationBuilder AddMessageBroker(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["MessageBroker:Host"], "/", h =>
                {
                    h.Username(builder.Configuration["MessageBroker:Username"]!);
                    h.Password(builder.Configuration["MessageBroker:Password"]!);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}
```

---

## Phase 4: REST API Endpoints

### 4.1 Auth Controller

```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResult), 200)]
    public async Task<ActionResult<AuthResult>> ValidateToken(ValidateTokenRequest request)
    {
        var result = await authService.ValidateTokenAsync(request.Token);
        return Ok(result);
    }

    [HttpGet("users/{userId:guid}")]
    [Authorize]  // Internal service auth
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserResponse>> GetUser(Guid userId)
    {
        var user = await authService.GetUserAsync(userId);
        if (user == null) return NotFound();
        return Ok(user);
    }
}
```

---

## Phase 5: Api Service Integration (Proxy Pattern)

### 5.1 How Proxy Works

The Whisprr.Api doesn't handle auth directly - it proxies to AuthService:

```
Client ──POST /api/auth/login (to Api)──▶ Api.AuthProxy ──POST /api/auth/login──▶ AuthService
                                              │                                      │
                                              │◀─────────AuthResponse───────────────┘
                                              │
                                              ▼
                                       Return token to client
```

### 5.2 Update Api.AuthProxy

```csharp
// Whisprr.Api/Auth/Proxy/IAuthProxy.cs (EXTENDED)
public interface IAuthProxy
{
    // Existing methods
    Task<AuthResult> ValidateTokenAsync(string token, CancellationToken ct = default);
    Task<UserInfo?> GetUserInfoAsync(Guid userId, CancellationToken ct = default);
    
    // NEW: Proxy login/register
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
}

// Whisprr.Api/Auth/Proxy/AuthProxy.cs (EXTENDED)
public partial class AuthProxy : IAuthProxy
{
    // Existing methods...
    
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Invalid response from auth service");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Invalid response from auth service");
    }
}
```

### 5.3 Optional: Auth Controller in Api

```csharp
// Whisprr.Api/Controllers/AuthController.cs (OPTIONAL - proxies to AuthService)
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthProxy authProxy) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Proxy to AuthService
        var response = await authProxy.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Proxy to AuthService
        var response = await authProxy.RegisterAsync(request);
        return Ok(response);
    }
}
```

**Benefits of proxy pattern:**
- Client only talks to Api (single endpoint)
- Api handles service discovery, retries, circuit breakers
- AuthService can be internal (not exposed externally)
- Consistent API surface for clients

### 5.4 Direct Access Alternative

Clients can also talk directly to AuthService if exposed:

```
Client ──POST /api/auth/login──▶ AuthService (direct)
```

This is simpler but:
- Client needs to know multiple service URLs
- Harder to enforce consistent auth policies
- CORS complexity

**Recommendation:** Use proxy pattern (Phase 5.3).

---

## Phase 6: Docker & Deployment

### 6.1 Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["services/Whisprr.AuthService/Whisprr.AuthService.csproj", "services/Whisprr.AuthService/"]
COPY ["lib/Whisprr.Contracts/Whisprr.Contracts.csproj", "lib/Whisprr.Contracts/"]
RUN dotnet restore "services/Whisprr.AuthService/Whisprr.AuthService.csproj"
COPY . .
WORKDIR "/src/services/Whisprr.AuthService"
RUN dotnet build "Whisprr.AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Whisprr.AuthService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Whisprr.AuthService.dll"]
```

### 6.2 docker-compose.yml Addition

```yaml
services:
  auth-service:
    build:
      context: .
      dockerfile: services/Whisprr.AuthService/Dockerfile
    ports:
      - "5001:80"
    environment:
      - ConnectionStrings__DefaultConnection=Host=auth-db;Database=whisprr_auth;Username=postgres;Password=postgres
      - Jwt__Key=${JWT_KEY}
      - MessageBroker__Host=rabbitmq
    depends_on:
      - auth-db
      - rabbitmq

  auth-db:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: whisprr_auth
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    volumes:
      - auth-db-data:/var/lib/postgresql/data
    ports:
      - "5433:5432"

volumes:
  auth-db-data:
```

---

## Implementation Checklist

### Phase 1: Setup
- [ ] Create project and add packages
- [ ] Create User domain model
- [ ] Setup AuthDbContext with EF Core
- [ ] Create initial migration
- [ ] Add configuration files

### Phase 2: Core Auth
- [ ] Implement ITokenService with JWT generation
- [ ] Implement IAuthService with register/login
- [ ] Add password hashing (BCrypt)
- [ ] Add refresh token support

### Phase 3: Message Broker
- [ ] Add MassTransit configuration
- [ ] Create UserCreatedEvent and UserLoggedInEvent (in Contracts)
- [ ] Implement IUserEventPublisher
- [ ] Publish events on register/login

### Phase 4: API
- [ ] Create AuthController with endpoints
- [ ] Add Swagger documentation
- [ ] Add input validation
- [ ] Add error handling middleware

### Phase 5: Api Integration
- [ ] Extend IAuthProxy in Api service
- [ ] Implement proxy methods for login/register
- [ ] Add AuthController to Api (optional proxy)
- [ ] Update configuration

### Phase 6: Deployment
- [ ] Create Dockerfile
- [ ] Update docker-compose.yml
- [ ] Add health checks
- [ ] Configure logging

---

## Security Considerations

1. **Password Security**
   - Use BCrypt with work factor 12+
   - Never store plain text passwords
   - Implement password strength requirements

2. **JWT Security**
   - Use strong symmetric key (32+ bytes)
   - Keep expiry short (15-60 minutes)
   - Use refresh tokens for extended sessions
   - Validate issuer, audience, signature

3. **Transport Security**
   - Use HTTPS in production
   - Secure cookies with HttpOnly, Secure, SameSite

4. **Rate Limiting**
   - Limit login attempts (prevent brute force)
   - Limit registration from same IP

5. **CORS**
   - Only allow specific origins
   - Don't allow credentials wildcard

---

## Next Steps

1. Implement Phase 1 (Project Setup)
2. Implement Phase 2 (Auth Services)
3. Implement Phase 3 (Message Broker)
4. Test locally with docker-compose
5. Implement Phase 5 (Api Integration)
6. Full end-to-end testing

Ready to start implementation?
