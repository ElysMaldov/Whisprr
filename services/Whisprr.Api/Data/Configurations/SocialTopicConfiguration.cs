using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Data.Configurations;

public class SocialTopicConfiguration : IEntityTypeConfiguration<SocialTopic>
{
    public void Configure(EntityTypeBuilder<SocialTopic> builder)
    {
        // Relationships and cascade delete are configured here
        // Other configurations are handled by data annotations
        
        builder.HasMany(t => t.Tasks)
            .WithOne(t => t.Topic)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(t => t.SocialInfos)
            .WithOne(i => i.Topic)
            .HasForeignKey(i => i.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(t => t.Subscriptions)
            .WithOne(s => s.Topic)
            .HasForeignKey(s => s.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
