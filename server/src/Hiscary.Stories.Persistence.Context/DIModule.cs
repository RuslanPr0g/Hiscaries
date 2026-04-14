using Hiscary.Stories.Persistence.Context.Seeds;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackNucleus.DDD.Persistence.EF.Postgres.Extensions;

namespace Hiscary.Stories.Persistence.Context;

public static class DIModule
{
    public static IServiceCollection AddStoriesPersistenceContext(this IServiceCollection services, IConfiguration configuration)
    {
        var registredServices = services.AddConfigurablePersistenceContext<StoriesContext>(
            configuration: configuration,
            migrationsAssemblyName: "Hiscary.Stories.Persistence.Context",
            migrationsHistoryTableSchemaName: StoriesContext.SCHEMA_NAME,
            configureDataSource: dataSourceBuilder => dataSourceBuilder.EnableDynamicJson());

        using (var scope = registredServices.BuildServiceProvider().CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StoriesContext>();
            GenreSeeder.SeedAsync(db);
        }

        return registredServices;
    }
}