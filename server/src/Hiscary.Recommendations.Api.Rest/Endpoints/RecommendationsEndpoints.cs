using Hiscary.Recommendations.Api.Rest.Requests;
using Hiscary.Recommendations.Domain.Queries;
using Hiscary.Recommendations.Domain.Services.Read;
using Microsoft.AspNetCore.Mvc;
using StackNucleus.DDD.Api.Rest;

namespace Hiscary.Recommendations.Api.Rest.Endpoints;

public static class RecommendationsEndpoints
{
    public static void MapRecommendationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/recommendations")
            .RequireAuthorization()
            .WithTags("Recommendations");

        group.MapPost("/stories", GetRecommendations)
            .Produces<IResult>(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetRecommendations(
        [FromBody] RecommendationsRequest request,
        [FromServices] IStorySearchService service,
        [FromServices] IAuthorizedEndpointHandler endpointHandler,
        CancellationToken cancellationToken) =>
        await endpointHandler.WithUser(user =>
            service.RecommendationsForUser(
                new StoryRecommendationsQuery()
                {
                    StartIndex = request.StartIndex,
                    ItemsCount = request.ItemsCount,
                    UserAccountId = user.Id,
                    SortAsc = false,
                    SortProperty = "DateWritten"
                },
                cancellationToken));
}
