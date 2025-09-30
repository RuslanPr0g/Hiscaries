using Hiscary.Recommendations.Domain.Services.Write;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Application.Write;

public static class DIModule
{
    public static IServiceCollection AddRecommendationsApplicationWriteLayer(this IServiceCollection services)
    {
        services.AddScoped<IUserPreferencesIndexService, UserPreferencesIndexService>();
        services.AddScoped<IStorySearchIndexService, StorySearchIndexService>();
        return services;
    }
}