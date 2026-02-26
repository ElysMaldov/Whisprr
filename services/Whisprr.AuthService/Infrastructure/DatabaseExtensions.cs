using Microsoft.EntityFrameworkCore;
using Whisprr.AuthService.Data;

namespace Whisprr.AuthService.Infrastructure;

/// <summary>
/// Extension methods for database registration and initialization.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Adds the database context to the application.
    /// </summary>
    public static IHostApplicationBuilder AddAuthDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            });
        });

        return builder;
    }


}
