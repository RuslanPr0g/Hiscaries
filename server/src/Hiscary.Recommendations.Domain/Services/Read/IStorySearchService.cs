using Hiscary.Recommendations.Domain.Entities;
using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Recommendations.Domain.Services.Read;

public interface IStorySearchService
{
    Task<ClientQueriedModel<Story>> RecommendationsForUser(Guid userId, CancellationToken ct = default);
}