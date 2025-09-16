using Hiscary.PlatformUsers.Domain;
using Hiscary.Shared.Persistence.EF.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.PlatformUsers.Persistence.Context.Configurations;

public class LibraryConfigurations : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("Libraries");
        builder.ConfigureEntity<Library, LibraryId, LibraryIdentityConverter>();

        builder.Property(s => s.AvatarImageUrls)
            .HasColumnName("AvatarImageUrls")
            .HasColumnType("jsonb")
            .HasImageContainerConversion();
    }
}
