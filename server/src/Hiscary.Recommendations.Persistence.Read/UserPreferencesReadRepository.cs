using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Persistence.Shared;

namespace Hiscary.Recommendations.Persistence.Read;

public class UserPreferencesReadRepository : IUserPreferencesReadRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchConfiguration _settings;

    public UserPreferencesReadRepository(ElasticsearchClient client, ElasticsearchConfiguration settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task<UserPreferences?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _client.GetAsync<UserPreferences>(id, g => g.Index(_settings.UserPreferencesIndex), ct);
        return response.Source;
    }
}