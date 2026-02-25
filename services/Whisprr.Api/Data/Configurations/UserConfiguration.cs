using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Partial index for ExternalAuthId (only where not null)
        builder.HasIndex(u => u.ExternalAuthId)
            .IsUnique()
            .HasFilter("\"ExternalAuthId\" IS NOT NULL");
        
        // Cascade delete for subscriptions
        builder.HasMany(u => u.Subscriptions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
