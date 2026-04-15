using Hiscary.PlatformUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.PlatformUsers.Persistence.Context.Configurations;

public class UserAnnotatedPdfConfigurations : IEntityTypeConfiguration<UserAnnotatedPdf>
{
    public void Configure(EntityTypeBuilder<UserAnnotatedPdf> builder)
    {
        builder.ToTable("UserAnnotatedPdfs");
        builder.ConfigureEntity();
        builder.HasKey(x => new { x.PlatformUserId, x.StoryId });
        builder.Property(x => x.PlatformUserId).HasConversion(new PlatformUserIdentityConverter());
        builder.Property(x => x.StoryId).IsRequired();
        builder.Property(x => x.PdfUrl).IsRequired();
        builder.Property(x => x.HasConflict).IsRequired();
    }
}
