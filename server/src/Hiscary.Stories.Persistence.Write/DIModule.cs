using Hiscary.Stories.Domain.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Stories.Persistence.Write;

public static class DIModule
{
    public static IServiceCollection AddStoriesPersistenceWriteLayer(
        this IServiceCollection services)
    {
        services.AddScoped<IStoryWriteRepository, StoryWriteRepository>();
        services.AddScoped<IGenreWriteRepository, GenreWriteRepository>();
        return services;
    }
}