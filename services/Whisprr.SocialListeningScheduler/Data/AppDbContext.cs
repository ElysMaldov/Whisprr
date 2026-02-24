using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Data;

internal class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
  {
  }

  public DbSet<DataSource> DataSources { get; set; } = null!;
  public DbSet<SocialTopic> SocialTopics { get; set; } = null!;
  public DbSet<SocialListeningTask> SocialListeningTasks { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Register PostgreSQL enum types for migrations
    modelBuilder.HasPostgresEnum<TaskProgressStatus>("scheduler");

    // SocialTopic: Keywords conversion (array <-> comma-separated string)
    modelBuilder.Entity<SocialTopic>(entity =>
    {
      entity.Property(e => e.Keywords)
          .HasConversion(
              v => string.Join(",", v),
              v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

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

      // Restrict delete when DataSource is deleted (tasks should be deleted first)
      entity.HasOne(e => e.DataSource)
          .WithMany(e => e.SocialListeningTasks)
          .OnDelete(DeleteBehavior.Restrict);
    });
  }
}
