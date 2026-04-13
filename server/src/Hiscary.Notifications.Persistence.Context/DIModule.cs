using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.Notifications.Persistence.Context;

public static class DIModule
{
    public static IServiceCollection AddNotificationsPersistenceContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddConfigurablePersistenceContext<NotificationsContext>(
            configuration: configuration,
            migrationsAssemblyName: "Hiscary.Notifications.Persistence.Context",
            migrationsHistoryTableSchemaName: NotificationsContext.SCHEMA_NAME);
    }
}