using Hiscary.Shared.Domain.Authorization;
using Hiscary.Shared.Domain.Extensions;
using Hiscary.Stories.Api.Rest.Requests.Comments;
using Hiscary.Stories.Api.Rest.Requests.Stories;
using Hiscary.Stories.Domain;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using Microsoft.AspNetCore.Mvc;
using StackNucleus.DDD.Api.Rest;
using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Stories.Api.Rest.Endpoints;

public static class StoryEndpoints
{
    public static void MapStoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/stories")
            .WithTags("Stories")
            .RequireAuthorization();

        group.MapPost("/search", GetStories)
            .Produces<IEnumerable<StoryWithContentsReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/by-id-with-contents", GetByIdWithContents)
            .Produces<IEnumerable<StoryWithContentsReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/by-ids", GetByIds)
            .Produces<IEnumerable<StorySimpleReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/owner", GetStoryOwner)
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/sortable-properties", GetSortableProperties)
            .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/genres", GetGenres)
            .Produces<IEnumerable<GenreReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/recommendations", BestToRead)
            .Produces<IEnumerable<StorySimpleReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/libraries/search", GetStoriesBy)
            .Produces<IEnumerable<StorySimpleReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/", PublishStory)
            .RequireAuthorization(AuthorizationPolicies.RequirePublisher)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPatch("/", UpdateStoryInformation)
            .RequireAuthorization(AuthorizationPolicies.RequirePublisher)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/comments", AddComment)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/score", ScoreStory)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete("/comments", DeleteComment)
            .RequireAuthorization(AuthorizationPolicies.RequireReaderOrAbove)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete("/", DeleteStory)
            .RequireAuthorization(AuthorizationPolicies.RequirePublisher)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPatch("/comments", UpdateComment)
            .RequireAuthorization(AuthorizationPolicies.RequireReaderOrAbove)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete("/audio", DeleteAudioForStory)
            .RequireAuthorization(AuthorizationPolicies.RequirePublisher)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetStoriesBy(
        [FromBody] GetStoryByLibraryIdRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(user =>
            service.SearchForStory(request.QueryableModel, user.Id, request.LibraryId));

    private static async Task<IResult> GetStories(
        [FromBody] GetStoryListRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(user =>
            service.SearchForStory(
                request.QueryableModel,
                storyId: request.Id,
                searchTerm: request.SearchTerm,
                genre: request.Genre));

    private static async Task<IResult> GetByIdWithContents(
        [FromBody] GetStoryWithContentsRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(user =>
            service.GetStoryById(request.Id));

    private static async Task<IResult> GetByIds(
        [FromBody] GetStoryByIdsRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(user =>
            service.GetStoryByIds(
                request.Ids.Select(_ => (StoryId)_).ToArray(),
                request.QueryableModel));

    private static IResult GetSortableProperties(
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
            endpointHandler.WithUser(user => StoryClientQueryableModelWithSortableRules.SortablePropertyList);

    private static async Task<IResult> GetGenres(
        [FromServices] IStoryReadService service) =>
        (await service.GetAllGenres()).ToHttpResult();

    private static async Task<IResult> BestToRead(
        [FromBody] StoryClientQueryableModelWithSortableRules model,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(async user =>
        {
            var result = await service.GetStoryRecommendations(model);
            // TODO: this is a temporary solution
            var stories = result.Items.ToList();
            return ClientQueriedModel<StorySimpleReadModel>.Create(
                stories.Shuffle(),
                result.TotalItemsCount,
                result.ItemsPerCurrentPageCount ?? stories.Count);
        });

    private static async Task<IResult> PublishStory(
        [FromBody] PublishStoryRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service) =>
        await endpointHandler.WithUserOperationValue(user =>
        {
            var image = request.ImagePreview.GetImageBytes();
            return service.PublishStory(
                user.Id,
                request.LibraryId,
                request.Title,
                request.Description,
                request.AuthorName,
                request.GenreIds,
                request.AgeLimit,
                image,
                true,
                request.DateWritten);
        });

    private static async Task<IResult> UpdateStoryInformation(
        [FromBody] StoryUpdateInfoRequest request,
        HttpContext httpContext,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service)
    {
        var callerRole = httpContext.User.FindFirst(AuthorizationPolicies.FullRoleClaimType)?.Value
            ?? httpContext.User.FindFirst(AuthorizationPolicies.RoleClaimType)?.Value
            ?? string.Empty;
        return await endpointHandler.WithUserOperation(user =>
        {
            var image = request.ImagePreview.GetImageBytes();
            return service.UpdateStory(
                user.Id,
                callerRole,
                request.StoryId,
                request.Title,
                request.Description,
                request.AuthorName,
                request.GenreIds,
                request.AgeLimit,
                image,
                request.ShouldUpdatePreview,
                request.PdfFileName,
                request.DateWritten,
                request.Contents);
        });
    }

    private static async Task<IResult> AddComment(
        [FromBody] CreateCommentRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service) =>
        await endpointHandler.WithUserOperation(user =>
            service.AddComment(
                request.StoryId,
                user.Id,
                request.Content,
                request.Score));

    private static async Task<IResult> ScoreStory(
        [FromBody] ScoreStoryRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service) =>
        await endpointHandler.WithUserOperation(user =>
            service.SetStoryScoreForAUser(
                request.StoryId,
                user.Id,
                request.Score));

    private static async Task<IResult> DeleteComment(
        Guid storyId,
        Guid commentId,
        HttpContext httpContext,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service)
    {
        var callerRole = httpContext.User.FindFirst(AuthorizationPolicies.FullRoleClaimType)?.Value
            ?? httpContext.User.FindFirst(AuthorizationPolicies.RoleClaimType)?.Value
            ?? string.Empty;
        return await endpointHandler.WithUserOperation(user =>
            service.DeleteComment(storyId, commentId, user.Id, callerRole));
    }

    private static async Task<IResult> DeleteStory(
        Guid storyId,
        HttpContext httpContext,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service)
    {
        var callerRole = httpContext.User.FindFirst(AuthorizationPolicies.FullRoleClaimType)?.Value
            ?? httpContext.User.FindFirst(AuthorizationPolicies.RoleClaimType)?.Value
            ?? string.Empty;
        return await endpointHandler.WithUserOperation(user =>
            service.DeleteStory(storyId, user.Id, callerRole));
    }

    private static async Task<IResult> UpdateComment(
        [FromBody] UpdateCommentRequest request,
        HttpContext httpContext,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service)
    {
        var callerRole = httpContext.User.FindFirst(AuthorizationPolicies.FullRoleClaimType)?.Value
            ?? httpContext.User.FindFirst(AuthorizationPolicies.RoleClaimType)?.Value
            ?? string.Empty;
        return await endpointHandler.WithUserOperation(user =>
            service.UpdateComment(
                request.CommentId,
                request.StoryId,
                request.Content,
                request.Score,
                user.Id,
                callerRole));
    }

    private static async Task<IResult> DeleteAudioForStory(
        Guid storyId,
        HttpContext httpContext,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service)
    {
        var callerRole = httpContext.User.FindFirst(AuthorizationPolicies.FullRoleClaimType)?.Value
            ?? httpContext.User.FindFirst(AuthorizationPolicies.RoleClaimType)?.Value
            ?? string.Empty;
        return await endpointHandler.WithUserOperation(user =>
            service.DeleteAudio(storyId, user.Id, callerRole));
    }

    private static async Task<IResult> GetStoryOwner(
        [FromQuery] Guid storyId,
        [FromServices] IStoryWriteRepository storyRepository)
    {
        var story = await storyRepository.GetById(storyId);
        if (story is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(story.LibraryId);
    }

}
