using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Data.Configurations;

public class SocialInfoConfiguration : IEntityTypeConfiguration<SocialInfo>
{
    public void Configure(EntityTypeBuilder<SocialInfo> builder)
    {
        // GIN index for JSONB querying (PostgreSQL-specific)
        builder.HasIndex(i => i.EngagementData)
            .HasMethod("GIN");
    }
}
