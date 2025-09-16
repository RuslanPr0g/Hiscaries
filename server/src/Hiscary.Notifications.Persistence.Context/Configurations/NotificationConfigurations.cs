using Hiscary.Notifications.Domain;
using Hiscary.Shared.Persistence.EF.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.Notifications.Persistence.Context.Configurations;

public class NotificationConfigurations : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.ConfigureEntity<Notification, NotificationId, NotificationIdentityConverter>();
        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.Type).IsRequired();
        builder.Property(c => c.Message).IsRequired();
        builder.Property(c => c.IsRead).IsRequired();
        builder.Property(c => c.RelatedObjectId);

        builder.Property(s => s.ImageUrls)
            .HasColumnName("ImageUrls")
            .HasColumnType("jsonb")
            .HasImageContainerConversion();
    }
}
