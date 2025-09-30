using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Queries;
using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Recommendations.Domain.Services.Read;

public interface IStorySearchService
{
    Task<ClientQueriedModel<Story>> RecommendationsForUser(StoryRecommendationsQuery query, CancellationToken ct = default);
}