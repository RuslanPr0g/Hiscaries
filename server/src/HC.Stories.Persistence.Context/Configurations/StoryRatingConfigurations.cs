﻿using Enterprise.Persistence.Context;
using HC.Stories.Domain.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HC.Stories.Persistence.Context.Configurations;

public class StoryRatingConfigurations : IEntityTypeConfiguration<StoryRating>
{
    public void Configure(EntityTypeBuilder<StoryRating> builder)
    {
        builder.ToTable("StoryRatings");
        builder.ConfigureEntity();
        builder.HasKey(sp => new { sp.StoryId, sp.PlatformUserId });
        builder.Property(c => c.PlatformUserId).IsRequired();
        builder.Property(c => c.StoryId).HasConversion(new StoryIdentityConverter());
    }
}
