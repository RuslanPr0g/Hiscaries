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

        group.MapGet("/", GetRecommendations)
            .Produces<IResult>(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetRecommendations(
        [FromServices] IStorySearchService service,
        IAuthorizedEndpointHandler endpointHandler,
        CancellationToken cancellationToken) =>
        await endpointHandler.WithUser(user =>
            service.RecommendationsForUser(user.Id, cancellationToken));
}
