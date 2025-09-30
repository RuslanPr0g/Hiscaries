using Hiscary.Recommendations.Domain.Persistence.Write;
using Hiscary.Recommendations.Persistence.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Persistence.Write;

public static class DIModule
{
    public static IServiceCollection AddRecommendationsPersistenceWriteLayer(
        this IServiceCollection services)
    {
        services.AddSingleton<ElasticsearchConfiguration>();
        services.AddScoped<IStorySearchIndexRepository, StorySearchIndexRepository>();
        services.AddScoped<IUserPreferencesIndexRepository, UserPreferencesIndexRepository>();
        return services;
    }
}