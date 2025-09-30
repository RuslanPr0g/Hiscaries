using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Application.Read;

public static class DIModule
{
    public static IServiceCollection AddRecommendationsApplicationReadLayer(this IServiceCollection services)
    {
        return services;
    }
}