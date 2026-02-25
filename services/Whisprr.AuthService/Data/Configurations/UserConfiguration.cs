using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisprr.AuthService.Models.Domain;

namespace Whisprr.AuthService.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.DisplayName)
            .HasMaxLength(200);
        
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(256);
    }
}
