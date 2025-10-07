using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Write;
using Hiscary.Recommendations.Persistence.Shared;

namespace Hiscary.Recommendations.Persistence.Write;

public class StorySearchIndexRepository : IStorySearchIndexRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchConfiguration _settings;

    public StorySearchIndexRepository(ElasticsearchClient client, ElasticsearchConfiguration settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task<IndexResponse> IndexAsync(Story entity, CancellationToken ct = default)
    {
        return await _client.IndexAsync(entity, i => i.Index(_settings.StoryIndex), ct);
    }

    public async Task<BulkResponse> IndexAsync(Story[] entities, CancellationToken ct = default)
    {
        return await _client.IndexManyAsync(entities, _settings.StoryIndex, ct);
    }

    public async Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _client.DeleteAsync<Story>(id, d => d.Index(_settings.StoryIndex), ct);
    }
}
