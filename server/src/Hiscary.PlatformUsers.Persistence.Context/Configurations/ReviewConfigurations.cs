using Hiscary.PlatformUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.PlatformUsers.Persistence.Context.Configurations;

public class ReviewConfigurations : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.ConfigureEntity();
        builder.HasKey(sp => new { sp.LibraryId, sp.PlatformUserId });
        builder.Property(c => c.LibraryId).HasConversion(new LibraryIdentityConverter());
        builder.Property(c => c.PlatformUserId).HasConversion(new PlatformUserIdentityConverter());
    }
}
