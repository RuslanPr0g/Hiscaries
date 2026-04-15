using Hiscary.Media.Application.Write.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Media.Application.Write;

public static class DIModule
{
    public static IServiceCollection AddMediaApplicationWriteLayer(this IServiceCollection services)
    {
        services.AddScoped<IMediaWriteService, MediaWriteService>();
        return services;
    }
}
