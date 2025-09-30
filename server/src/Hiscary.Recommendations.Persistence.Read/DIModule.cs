using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Persistence.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Persistence.Read;

public static class DIModule
{
    public static IServiceCollection AddRecommendationsPersistenceReadLayer(
        this IServiceCollection services)
    {
        services.AddSingleton<ElasticsearchConfiguration>();
        services.AddScoped<IStorySearchRepository, StorySearchRepository>();
        services.AddScoped<IUserPreferencesReadRepository, UserPreferencesReadRepository>();
        return services;
    }
}