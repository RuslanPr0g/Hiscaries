using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Persistence.Write;

public interface IUserPreferencesIndexRepository
{
    Task<IndexResponse> IndexAsync(UserPreferences entity, CancellationToken ct = default);
    Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default);
}
