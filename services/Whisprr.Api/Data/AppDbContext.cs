using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<SocialTopic> SocialTopics => Set<SocialTopic>();
    public DbSet<SocialListeningTask> SocialListeningTasks => Set<SocialListeningTask>();
    public DbSet<SocialInfo> SocialInfos => Set<SocialInfo>();
    public DbSet<UserTopicSubscription> UserTopicSubscriptions => Set<UserTopicSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
