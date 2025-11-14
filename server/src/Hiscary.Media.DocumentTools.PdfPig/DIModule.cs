using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Media.DocumentTools.PdfPig;

public static class DIModule
{
    public static IServiceCollection AddPdfDocumentTools(this IServiceCollection services)
    {
        services.AddScoped<IDocumentTool, PdfDocumentTool>();
        return services;
    }
}