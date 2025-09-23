    using Hiscary.PlatformUsers.Api.Rest.Requests;
using Hiscary.PlatformUsers.Api.Rest.Requests.Libraries;
using Hiscary.PlatformUsers.Domain.ProcessModels;
using Hiscary.PlatformUsers.Domain.Services;
using Hiscary.Shared.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using StackNucleus.DDD.Api.Rest;
using StackNucleus.DDD.Domain.ResultModels;

namespace Hiscary.PlatformUsers.Api.Rest.Endpoints;

public static class PlatformUserEndpoints
{
    public static void MapPlatformUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("Users");

        group.MapPost("/story-metadata", GetUserReadingStoryMetadata)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/resume-reading", ResumeReading)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/reading-history", ReadingHistory)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/libraries", GetLibrary)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/libraries", EditLibrary)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/libraries/subscriptions/subscribe", SubscribeToLibrary)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/libraries/subscriptions/unsubscribe", UnsubscribeFromLibrary)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/read", ReadStory)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/bookmark", BookmarkStory)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/become-publisher", BecomePublisher)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> BookmarkStory(
        [FromBody] BookmarkStoryRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserWriteService service) =>
        await endpointHandler.WithUser(user =>
            service.BookmarkStory(user.Id, request.StoryId));

    private static async Task<IResult> ReadStory(
        [FromBody] ReadStoryRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserWriteService service) =>
        await endpointHandler.WithUser((Func<CurrentUser, Task<OperationResult>>)(user =>
            service.ReadStoryPage(user.Id, request.StoryId, request.PageRead)));

    private static async Task<IResult> BecomePublisher(
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserWriteService service) =>
        await endpointHandler.WithUser(user =>
            service.BecomePublisher(user.Id));

    private static async Task<IResult> GetLibrary(
        [FromQuery] Guid? libraryId,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserReadService service) =>
        await endpointHandler.WithUser(user =>
            service.GetLibraryInformation(user.Id, libraryId));

    private static async Task<IResult> ResumeReading(
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserReadService service) =>
        await endpointHandler.WithUser(user =>
            service.GetResumeReadingStoryIds(user.Id));

    private static async Task<IResult> ReadingHistory(
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserReadService service) =>
        await endpointHandler.WithUser(user =>
            service.GetReadingHistoryStoryIds(user.Id));

    private static async Task<IResult> GetUserReadingStoryMetadata(
        [FromBody] UserReadingStoryRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserReadService service) =>
        await endpointHandler.WithUser(user =>
            service.GetUserReadingStoryMetadata(
                user.Id,
                IsEmpty(request?.Items) ? [] :
                request!.Items.Select(_ =>
                new UserReadingStoryProcessModel
                {
                    StoryId = _.StoryId,
                    LibraryId = _.LibraryId,
                    TotalPages = _.TotalPages
                }).ToArray()));

    private static async Task<IResult> EditLibrary(
        [FromBody] EditLibraryRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserWriteService service) =>
        await endpointHandler.WithUser(user =>
        {
            var image = request.Avatar.GetImageBytes();
            return service.EditLibraryInfo(
                user.Id,
                request.LibraryId,
                request.Bio,
                image,
                request.ShouldUpdateAvatar,
                request.LinksToSocialMedia);
        });

    private static async Task<IResult> SubscribeToLibrary(
        [FromBody] LibrarySubscriptionRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserWriteService service) =>
        await endpointHandler.WithUser(user =>
            service.SubscribeToLibrary(user.Id, request.LibraryId));

    private static async Task<IResult> UnsubscribeFromLibrary(
        [FromBody] LibrarySubscriptionRequest request,
        IAuthorizedEndpointHandler endpointHandler,
        [FromServices] IPlatformUserWriteService service) =>
        await endpointHandler.WithUser(user =>
            service.UnsubscribeFromLibrary(user.Id, request.LibraryId));

    private static bool IsEmpty<T>(IEnumerable<T>? items)
    {
        return items is null || !items.Any();
    }
}
