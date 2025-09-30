using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Write;
using Hiscary.Recommendations.Domain.Services.Write;

namespace Hiscary.Recommendations.Application.Write;

internal sealed class StorySearchIndexService : IStorySearchIndexService
{
    private readonly IStorySearchIndexRepository _repository;

    public StorySearchIndexService(IStorySearchIndexRepository repository)
    {
        _repository = repository;
    }

    public async Task AddOrUpdateAsync(Story entity, CancellationToken ct = default)
    {
        await _repository.IndexAsync(entity, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(id, ct);
    }
}
