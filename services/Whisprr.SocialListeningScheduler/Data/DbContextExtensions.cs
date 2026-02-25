using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Data.Seeding;
using Whisprr.SocialListeningScheduler.Options;

namespace Whisprr.SocialListeningScheduler.Data;

public static class DbContextExtensions
{
  public static IHostApplicationBuilder AddAppDbContext(this IHostApplicationBuilder builder)
  {
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
      options.UseNpgsql(connectionString, npgsqlOptions =>
      {
        npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName); // Sets migration in the root folder of this project

        // Map enums to PostgreSQL native enum types
        npgsqlOptions.MapEnum<TaskProgressStatus>();
        npgsqlOptions.MapEnum<PlatformType>();
      });
    });

    return builder;
  }

  public static async Task SeedDatabaseAsync(this IHost host)
  {
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    var seedingOptions = configuration.GetSection(SeedingOptions.SectionName).Get<SeedingOptions>();

    if (seedingOptions?.RunOnStartup != true)
    {
      return;
    }

    using var scope = host.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensure database is created and migrations are applied
    await dbContext.Database.MigrateAsync();

    // Trigger async seeding by calling the seeding method directly
    await AppDbContextSeeder.SeedAsync(dbContext);
  }
}
