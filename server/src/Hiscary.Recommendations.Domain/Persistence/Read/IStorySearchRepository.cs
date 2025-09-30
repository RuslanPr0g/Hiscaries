using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Queries;
using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Recommendations.Domain.Persistence.Read;

public interface IStorySearchRepository
{
    Task<Story?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ClientQueriedModel<Story>> SearchAsync(SearchStoryQuery query, CancellationToken ct = default);
    Task<ClientQueriedModel<Story>> RecommendationsForUser(Guid userId, CancellationToken ct = default);
}
