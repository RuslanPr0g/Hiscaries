using Microsoft.EntityFrameworkCore;
using StackNucleus.DDD.Persistence.EF.Postgres;

namespace Hiscary.Notifications.Persistence.Context;

public class NotificationsDatabaseDesignTimeDbContextFactory
    : NucleusDatabaseDesignTimeDbContextFactory<NotificationsContext>
{
    public override NotificationsContext CreateDbContextBasedOnOptions(
        DbContextOptions<NotificationsContext> options)
    {
        return new NotificationsContext(options);
    }
}
