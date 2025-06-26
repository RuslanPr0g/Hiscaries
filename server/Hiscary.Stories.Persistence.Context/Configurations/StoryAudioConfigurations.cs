﻿using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;
using Hiscary.Stories.Domain.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiscary.Stories.Persistence.Context.Configurations;

public class StoryAudioConfigurations : IEntityTypeConfiguration<StoryAudio>
{
    public void Configure(EntityTypeBuilder<StoryAudio> builder)
    {
        builder.ToTable("StoryAudios");
        builder.ConfigureEntity<StoryAudio, StoryAudioId, StoryAudioIdentityConverter>();
    }
}
