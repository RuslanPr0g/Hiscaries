﻿using Hiscary.Api.Rest;
using StackNucleus.DDD.Domain.Extensions;
using Hiscary.Stories.Api.Rest.Requests.Comments;
using Hiscary.Stories.Api.Rest.Requests.Stories;
using Hiscary.Stories.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using Microsoft.AspNetCore.Mvc;

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

        group.MapGet("/sortable-properties", GetSortableProperties)
            .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/genres", GetGenres)
            .Produces<IEnumerable<GenreReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/recommendations", BestToRead)
            .Produces<IEnumerable<StorySimpleReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/", GetStoriesBy)
            .Produces<IEnumerable<StorySimpleReadModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/", PublishStory)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPatch("/", UpdateStoryInformation)
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
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete("/", DeleteStory)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPatch("/comments", UpdateComment)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete("/audio", DeleteAudioForStory)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetStoriesBy(
        [FromQuery] Guid libraryId,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(user =>
            service.SearchForStory(user.Id, libraryId));

    private static async Task<IResult> GetStories(
        [FromBody] GetStoryListRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(user =>
            service.SearchForStory(
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
                request.Sorting?.Property ?? "CreatedAt",
                request.Sorting?.Ascending ?? true));

    private static IResult GetSortableProperties(
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
            endpointHandler.WithUser(user => service.SortableProperties);

    private static async Task<IResult> GetGenres(
        [FromServices] IStoryReadService service) =>
        (await service.GetAllGenres()).ToHttpResult();

    private static async Task<IResult> BestToRead(
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryReadService service) =>
        await endpointHandler.WithUser(async user =>
        {
            var result = await service.GetStoryRecommendations();
            var response = result.ToList();
            response.Shuffle();
            return response;
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
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IStoryWriteService service) =>
        await endpointHandler.WithUserOperation(user =>
        {
            var image = request.ImagePreview.GetImageBytes();
            return service.UpdateStory(
                user.Id,
                request.StoryId,
                request.Title,
                request.Description,
                request.AuthorName,
                request.GenreIds,
                request.AgeLimit,
                image,
                request.ShouldUpdatePreview,
                request.DateWritten,
                request.Contents);
        });

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
        [FromServices] IStoryWriteService service) =>
        (await service.DeleteComment(storyId, commentId)).OperationToHttpResult();

    private static async Task<IResult> DeleteStory(
        Guid storyId,
        [FromServices] IStoryWriteService service) =>
        (await service.DeleteStory(storyId)).OperationToHttpResult();

    private static async Task<IResult> UpdateComment(
        [FromBody] UpdateCommentRequest request,
        [FromServices] IStoryWriteService service) =>
        (await service.UpdateComment(
            request.CommentId,
            request.StoryId,
            request.Content,
            request.Score)).OperationToHttpResult();

    private static async Task<IResult> DeleteAudioForStory(
        Guid storyId,
        [FromServices] IStoryWriteService service) =>
        (await service.DeleteAudio(storyId)).OperationToHttpResult();

}
