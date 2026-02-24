using Microsoft.EntityFrameworkCore;
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

    // SocialListeningTask: Enum-to-string conversion for Status
    modelBuilder.Entity<SocialListeningTask>(entity =>
    {
      entity.Property(e => e.Status)
          .HasConversion<string>();

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
