using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisprr.Api.Models.Domain;

namespace Whisprr.Api.Data.Configurations;

public class UserTopicSubscriptionConfiguration : IEntityTypeConfiguration<UserTopicSubscription>
{
    public void Configure(EntityTypeBuilder<UserTopicSubscription> builder)
    {
        // Table name and relationships only
        // Indexes and constraints are handled by data annotations
        builder.ToTable("UserTopicSubscriptions");
    }
}
