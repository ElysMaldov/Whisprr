using Microsoft.EntityFrameworkCore;
using Whisprr.AuthService.Models.Domain;

namespace Whisprr.AuthService.Data;

/// <summary>
/// Database context for authentication-related entities.
/// </summary>
public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }
}
