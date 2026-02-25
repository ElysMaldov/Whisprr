using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Data.Configurations;

public class SocialListeningTaskConfiguration : IEntityTypeConfiguration<SocialListeningTask>
{
    public void Configure(EntityTypeBuilder<SocialListeningTask> builder)
    {
        // Enum to string conversion (not supported by data annotations)
        builder.Property(t => t.Status)
            .HasConversion<string>();
        
        // Relationship with restrict delete to prevent cascade cycles
        builder.HasMany(t => t.SocialInfos)
            .WithOne(i => i.Task)
            .HasForeignKey(i => i.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
