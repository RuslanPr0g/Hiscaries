using Hiscary.PlatformUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.PlatformUsers.Persistence.Context.Configurations;

public class UserReadHistoryConfigurations : IEntityTypeConfiguration<ReadingHistory>
{
    public void Configure(EntityTypeBuilder<ReadingHistory> builder)
    {
        builder.ToTable("ReadingHistories");
        builder.ConfigureEntity();
        builder.HasKey(sp => new { sp.StoryId, sp.PlatformUserId });
        builder.Property(c => c.PlatformUserId).HasConversion(new PlatformUserIdentityConverter());
        builder.Property(c => c.StoryId).IsRequired();
    }
}
