using Hiscary.Media.DocumentTools;
using Hiscary.Shared.Domain.FileStorage;
using Microsoft.AspNetCore.Mvc;

namespace Hiscary.Media.Api.Rest.Endpoints;

public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/media")
            .WithTags("Media");

        group.MapGet("/images/{fileName}", GetImage)
            .Produces<IResult>(StatusCodes.Status200OK, contentType: "application/octet-stream")
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/document/as-contents", GetDocumentAsContents)
            .Produces<IResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetImage(
        [FromRoute] string fileName,
        [FromServices] IBlobStorageService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var (content, contentType) = await service.DownloadAsync(
                "images",
                blobName: fileName,
                cancellationToken: cancellationToken);

            return Results.File(content, contentType);
        }
        catch (FileNotFoundException)
        {
            return Results.NotFound();
        }
    }

    public static async Task<IResult> GetDocumentAsContents(
        [FromServices] HttpRequest request,
        [FromServices] IDocumentTool documentTool)
    {
        // TODO: for now, we support PDF only, for the future we would require a strategy or smth for the document tool.
        if (!request.Form.Files.Any())
            return Results.BadRequest("No file uploaded.");

        var file = request.Form.Files[0];

        if (file.Length == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }

        if (!file.Name.EndsWith(".pdf"))
        {
            return Results.UnprocessableEntity("The document format cannot be processed. We support PDF only.");
        }

        using var stream = file.OpenReadStream();

        var documentContent = documentTool.FileStreamToContent(stream);

        if (documentContent.Pages.Count == 0)
        {
            return Results.UnprocessableEntity("The document format cannot be processed. We support PDF only.");
        }

        return Results.Json(documentContent, statusCode: 200);
    }
}
