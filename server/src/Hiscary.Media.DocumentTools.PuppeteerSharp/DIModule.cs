using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Media.DocumentTools.PuppeteerSharp;

public static class DIModule
{
    public static IServiceCollection AddPuppeteerPdfGenerator(this IServiceCollection services)
    {
        services.AddScoped<IPdfGenerator, PuppeteerPdfGenerator>();
        return services;
    }
}
