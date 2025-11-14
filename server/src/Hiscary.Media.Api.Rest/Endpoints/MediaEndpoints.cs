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

        group.MapPost("/documents/as-contents", GetDocumentAsContents)
            // TODO: for now only pdf
            .Accepts<Stream>("application/pdf")
            .Produces<IResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity);
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
        HttpRequest request,
        [FromServices] IDocumentTool documentTool,
        [FromQuery] int? start,
        [FromQuery] int? end)
    {
        // TODO: for now only pdf
        if (!request.ContentType?.StartsWith("application/pdf") ?? true)
        {
            return Results.BadRequest("Please upload a valid PDF file in the request body.");
        }

        await using var stream = new MemoryStream();
        await request.Body.CopyToAsync(stream);
        stream.Position = 0;

        var documentContent = documentTool.FileStreamToContent(stream, start, end);

        if (documentContent.Pages.Count == 0)
        {
            return Results.UnprocessableEntity("The PDF has no pages or cannot be processed.");
        }

        return Results.Json(documentContent, statusCode: 200);
    }
}
