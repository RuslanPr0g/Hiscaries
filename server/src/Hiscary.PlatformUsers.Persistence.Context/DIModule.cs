using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.PlatformUsers.Persistence.Context;

public static class DIModule
{
    public static IServiceCollection AddPlatformUsersPersistenceContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddConfigurablePersistenceContext<PlatformUsersContext>(
            configuration: configuration,
            migrationsAssemblyName: "Hiscary.PlatformUsers.Persistence.Context",
            migrationsHistoryTableSchemaName: PlatformUsersContext.SCHEMA_NAME);
    }
}