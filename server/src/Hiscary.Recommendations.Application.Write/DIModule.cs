using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Application.Write;

public static class DIModule
{
    public static IServiceCollection AddRecommendationsApplicationWriteLayer(this IServiceCollection services)
    {
        return services;
    }
}