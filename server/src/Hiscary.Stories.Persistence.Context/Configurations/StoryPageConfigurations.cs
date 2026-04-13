using Hiscary.Stories.Domain.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.Stories.Persistence.Context.Configurations;

public class StoryPageConfigurations : IEntityTypeConfiguration<StoryPage>
{
    public void Configure(EntityTypeBuilder<StoryPage> builder)
    {
        builder.ToTable("StoryPages");
        builder.ConfigureEntity();
        builder.HasKey(sp => new { sp.StoryId, sp.Page });
        builder.Property(c => c.StoryId).HasConversion(new StoryIdentityConverter());
    }
}
