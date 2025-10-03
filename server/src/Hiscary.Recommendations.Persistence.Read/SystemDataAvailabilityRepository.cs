using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Persistence.Shared;

namespace Hiscary.Recommendations.Persistence.Read;

internal sealed class SystemDataAvailabilityRepository : ISystemDataAvailabilityRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchConfiguration _settings;

    public SystemDataAvailabilityRepository(ElasticsearchClient client, ElasticsearchConfiguration settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task CreateUserIndex(CancellationToken ct = default)
    {
        await _client.Indices.CreateAsync(new CreateIndexRequest(_settings.UserPreferencesIndex), ct);
    }

    public async Task CreateStoryIndex(CancellationToken ct = default)
    {
        await _client.Indices.CreateAsync(new CreateIndexRequest(_settings.StoryIndex), ct);
    }

    public async Task<bool> IsUserDataAvailable(CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<UserPreferences>(s => s
                .Index(_settings.UserPreferencesIndex)
                .From(0)
                .Size(1)
                .Query(q => q.MatchAll(new MatchAllQuery()))
            , ct);

        return response.IsValidResponse && response.IsSuccess();
    }

    public async Task<bool> IsStoryDataAvailable(CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<Story>(s => s
                .Index(_settings.StoryIndex)
                .From(0)
                .Size(1)
                .Query(q => q.MatchAll(new MatchAllQuery()))
            , ct);

        return response.IsValidResponse && response.IsSuccess();
    }
}