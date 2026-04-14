using Hiscary.Media.DocumentTools;
using Hiscary.Shared.Domain.FileStorage;
using Microsoft.AspNetCore.Mvc;
using StackNucleus.DDD.Domain.ResultModels;
using System.Net.Mime;

namespace Hiscary.Media.Api.Rest.Endpoints;

// TODO: add proper RBAC
public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/media")
            .WithTags("Media");

        group.MapGet("/images/{fileName}", GetImage)
            .Produces<IResult>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Octet)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/documents/{fileName}", GetDocument)
            .Produces<IResult>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Pdf)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/documents/as-contents", GetDocumentAsContents)
            .RequireAuthorization()
            .Accepts<Stream>(MediaTypeNames.Application.Pdf)
            .Produces<IResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity);

        group.MapPost("/documents/upload", UploadDocument)
            .Accepts<Stream>(MediaTypeNames.Application.Pdf)
            .Produces<IResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity);

        group.MapDelete("/documents", DeleteDocument)
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

    private static async Task<IResult> GetDocument(
        [FromRoute] string fileName,
        [FromServices] IBlobStorageService service,
        CancellationToken cancellationToken)
    {
        try
        {
            var (content, contentType) = await service.DownloadAsync(
                "documents",
                blobName: fileName,
                cancellationToken: cancellationToken);

            return Results.File(content, contentType);
        }
        catch (FileNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> GetDocumentAsContents(
        HttpRequest request,
        [FromServices] IDocumentTool documentTool,
        [FromQuery] int? start,
        [FromQuery] int? end)
    {
        if (!request.ContentType?.StartsWith(MediaTypeNames.Application.Pdf) ?? true)
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

    private static async Task<IResult> UploadDocument(
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] IBlobStorageService storageService,
        [FromQuery] Guid storyId,
        [FromQuery] int? start,
        [FromQuery] int? end)
    {
        if (!request.ContentType?.StartsWith(MediaTypeNames.Application.Pdf) ?? true)
        {
            return Results.BadRequest(OperationResult.CreateValidationsError("Please upload a valid PDF file in the request body."));
        }

        if (storyId == default || storyId == Guid.Empty)
        {
            return Results.BadRequest(OperationResult.CreateValidationsError("Please provide story id to upload PDF for."));
        }

        var token = httpContext.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            return Results.Unauthorized();
        }

        await using var stream = new MemoryStream();
        await request.Body.CopyToAsync(stream);
        stream.Position = 0;
        var bytes = stream.ToArray();

        var link = await storageService.UploadAsync(
            "documents",
            BuildFileName(storyId, "pdf"),
            bytes,
            MediaTypeNames.Application.Pdf);

        if (!link.HasValue || string.IsNullOrWhiteSpace(link.Value))
        {
            return Results.UnprocessableEntity("The PDF has no pages or cannot be processed.");
        }

        return Results.Ok(OperationResult.CreateSuccess());
    }

    private static async Task<IResult> DeleteDocument(
        HttpContext httpContext,
        [FromServices] IBlobStorageService storageService,
        [FromQuery] Guid storyId)
    {
        if (storyId == default || storyId == Guid.Empty)
        {
            return Results.BadRequest(OperationResult.CreateValidationsError("Please provide story id to delete PDF for."));
        }

        var token = httpContext.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            return Results.Unauthorized();
        }

        var result = await storageService.DeleteAsync(
            "documents",
            BuildFileName(storyId, "pdf"));

        if (!result.HasValue || !result.Value)
        {
            return Results.UnprocessableEntity(OperationResult.CreateValidationsError("Failed to delete document."));
        }

        return Results.Ok(OperationResult.CreateSuccess());
    }

    private static string BuildFileName(Guid fileId, string extension)
    {
        return $"{fileId}.{extension}";
    }
}
