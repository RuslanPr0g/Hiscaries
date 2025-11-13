using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Persistence.Shared;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Hiscary.Recommendations.Persistence.Read;

internal sealed class SystemDataAvailabilityRepository : ISystemDataAvailabilityRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchConfiguration _settings;
    private readonly AsyncRetryPolicy<bool> _retryPolicy;
    private readonly ILogger<SystemDataAvailabilityRepository> _logger;

    public SystemDataAvailabilityRepository(
        ElasticsearchClient client,
        ElasticsearchConfiguration settings,
        ILogger<SystemDataAvailabilityRepository> logger)
    {
        _client = client;
        _settings = settings;
        _logger = logger;

        _retryPolicy = Policy
            .HandleResult<bool>(r => r == false)
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(2),
                onRetry: (outcome, timespan, retryCount, _) =>
                {
                    _logger.LogWarning("Retry {RetryCount} after {Delay}s due to unavailable data.", retryCount, timespan.TotalSeconds);
                });
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
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.SearchAsync<UserPreferences>(s => s
                .Indices(_settings.UserPreferencesIndex)
                .From(0)
                .Size(1)
                .Query(q => q.MatchAll(new MatchAllQuery())), ct);

            if (!response.IsValidResponse || !response.IsSuccess())
            {
                return false;
            }

            return response.Total > 0;
        });
    }

    public async Task<bool> IsStoryDataAvailable(CancellationToken ct = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.SearchAsync<Story>(s => s
                .Indices(_settings.StoryIndex)
                .From(0)
                .Size(1)
                .Query(q => q.MatchAll(new MatchAllQuery())), ct);

            if (!response.IsValidResponse || !response.IsSuccess())
            {
                return false;
            }

            return response.Total > 0;
        });
    }
}
