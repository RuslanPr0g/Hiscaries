using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Domain.Services.Read;
using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Recommendations.Application.Read;

internal sealed class StorySearchService : IStorySearchService
{
    private readonly IStorySearchRepository _repository;

    public StorySearchService(IStorySearchRepository repository)
    {
        _repository = repository;
    }

    public Task<ClientQueriedModel<Story>> RecommendationsForUser(Guid userId, CancellationToken ct = default) => _repository.RecommendationsForUser(userId, ct);
}
