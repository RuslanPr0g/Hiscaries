using Hiscary.Media.Images.Uploaders;
using Microsoft.Extensions.DependencyInjection;
using StackNucleus.DDD.Domain.Images.Uploaders;

namespace Hiscary.Media.Images;

public static class DIModule
{
    public static IServiceCollection AddMediaImages(this IServiceCollection services)
    {
        services.AddScoped<IImageUploader, ImageUploader>();
        return services;
    }
}