using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Write;
using Hiscary.Recommendations.Persistence.Shared;

namespace Hiscary.Recommendations.Persistence.Write;

public class UserPreferencesIndexRepository : IUserPreferencesIndexRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchConfiguration _settings;

    public UserPreferencesIndexRepository(ElasticsearchClient client, ElasticsearchConfiguration settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task<IndexResponse> IndexAsync(UserPreferences entity, CancellationToken ct = default)
    {
        return await _client.IndexAsync(entity, i => i.Index(_settings.UserPreferencesIndex), ct);
    }

    public async Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _client.DeleteAsync<Story>(id, d => d.Index(_settings.UserPreferencesIndex), ct);
    }
}