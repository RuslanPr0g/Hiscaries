using Elastic.Clients.Elasticsearch;
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

    public async Task<IndexResponse> AddOrUpdateAsync(Story entity, CancellationToken ct = default)
    {
        return await _repository.IndexAsync(entity, ct);
    }

    public async Task<BulkResponse> AddOrUpdateAsync(Story[] entities, CancellationToken ct = default)
    {
        return await _repository.IndexAsync(entities, ct);
    }

    public async Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(id, ct);
    }
}
