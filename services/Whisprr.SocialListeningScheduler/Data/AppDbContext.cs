using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Data;

internal class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
  {
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
  }

  public DbSet<SocialTopic> SocialTopics { get; set; } = null!;
  public DbSet<SocialListeningTask> SocialListeningTasks { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Add MassTransit Transactional Outbox entities (OutboxMessage, OutboxState, InboxState)
    // This enables the Transactional Outbox pattern for reliable message delivery
    modelBuilder.AddTransactionalOutboxEntities();

    // Register PostgreSQL enum types for migrations
    modelBuilder.HasPostgresEnum<TaskProgressStatus>();
    modelBuilder.HasPostgresEnum<PlatformType>();

    // SocialTopic: Keywords conversion (array <-> comma-separated string)
    modelBuilder.Entity<SocialTopic>(entity =>
    {
      entity.Property(e => e.Keywords)
          .HasConversion(
              v => string.Join(",", v),
              v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
          .Metadata.SetValueComparer(new ValueComparer<string[]>(
              (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
              c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
              c => c == null ? Array.Empty<string>() : c.ToArray()));

      // Language conversion (CultureInfo <-> BCP-47 string)
      entity.Property(e => e.Language)
          .HasConversion(
              v => v.Name,
              v => new System.Globalization.CultureInfo(v));
    });

    // SocialListeningTask configuration
    modelBuilder.Entity<SocialListeningTask>(entity =>
    {
      // Cascade delete when SocialTopic is deleted
      entity.HasOne(e => e.SocialTopic)
          .WithMany(e => e.SocialListeningTasks)
          .OnDelete(DeleteBehavior.Cascade);
    });
  }
}
