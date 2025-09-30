using Hiscary.Recommendations.Domain.Services.Read;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Application.Read;

public static class DIModule
{
    public static IServiceCollection AddRecommendationsApplicationReadLayer(this IServiceCollection services)
    {
        services.AddScoped<IStorySearchService, StorySearchService>();
        return services;
    }
}