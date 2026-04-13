using Hiscary.Notifications.Domain;
using StackNucleus.DDD.Persistence.EF.Postgres.Configurations;

namespace Hiscary.Notifications.Persistence.Context.Configurations;

public class NotificationIdentityConverter : IdentityConverter<NotificationId>
{
    public NotificationIdentityConverter() :
        base((x) => new NotificationId(x))
    {
    }
}
