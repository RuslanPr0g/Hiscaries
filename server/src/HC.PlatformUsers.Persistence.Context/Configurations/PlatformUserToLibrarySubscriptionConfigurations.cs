﻿using Enterprise.Persistence.Context.Extensions;
using HC.PlatformUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HC.PlatformUsers.Persistence.Context.Configurations;

public class PlatformUserToLibrarySubscriptionConfigurations : IEntityTypeConfiguration<PlatformUserToLibrarySubscription>
{
    public void Configure(EntityTypeBuilder<PlatformUserToLibrarySubscription> builder)
    {
        builder.ToTable("PlatformUserToLibrarySubscription");
        builder.ConfigureEntity();
        builder.HasKey(sp => new { sp.LibraryId, sp.PlatformUserId });
        builder.Property(c => c.LibraryId).HasConversion(new LibraryIdentityConverter());
        builder.Property(c => c.PlatformUserId).HasConversion(new PlatformUserIdentityConverter());

        builder
            .HasOne(uls => uls.PlatformUser)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(uls => uls.PlatformUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
