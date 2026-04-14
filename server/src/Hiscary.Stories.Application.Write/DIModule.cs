using Hiscary.Stories.Application.Write.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Stories.Application.Write;

public static class DIModule
{
    public static IServiceCollection AddStoriesApplicationWriteLayer(this IServiceCollection services)
    {
        services.AddScoped<IStoryWriteService, StoryWriteService>();
        return services;
    }
}