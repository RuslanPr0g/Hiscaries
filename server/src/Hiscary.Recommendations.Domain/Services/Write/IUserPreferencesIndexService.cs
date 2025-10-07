using Elastic.Clients.Elasticsearch;

namespace Hiscary.Recommendations.Domain.Services.Write;

public interface IUserPreferencesIndexService
{
    Task<IndexResponse?> AddOrUpdateAsync(Guid userAccountId, Guid storyId, CancellationToken ct = default);
    Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default);
}