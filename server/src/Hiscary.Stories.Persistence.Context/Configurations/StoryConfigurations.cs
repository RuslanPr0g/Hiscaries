using Hiscary.Shared.Persistence.EF.Postgres;
using Hiscary.Stories.Domain.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.Stories.Persistence.Context.Configurations;

public class StoryConfigurations : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("Stories");
        builder.ConfigureEntity<Story, StoryId, StoryIdentityConverter>();
        builder.Property(c => c.LibraryId).IsRequired();

        builder
            .HasMany(s => s.Genres)
            .WithMany()
            .UsingEntity(j => j.ToTable("StoryGenres"));

        builder
            .HasMany(s => s.Contents)
            .WithOne()
            .HasForeignKey(sp => sp.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Comments)
            .WithOne(c => c.Story)
            .HasForeignKey(c => c.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Audios)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Ratings)
            .WithOne()
            .HasForeignKey(sr => sr.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.ImagePreviewUrl)
            .HasColumnName("ImagePreviewUrl")
            .HasColumnType("jsonb")
            .HasImageContainerConversion();

        builder.Navigation(x => x.Genres).AutoInclude();
        builder.Navigation(x => x.Ratings).AutoInclude();
        builder.Navigation(x => x.Comments).AutoInclude();
        builder.Navigation(x => x.Contents).AutoInclude();
    }
}
