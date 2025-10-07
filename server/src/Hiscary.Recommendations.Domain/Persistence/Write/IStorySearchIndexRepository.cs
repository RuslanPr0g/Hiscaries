using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Persistence.Write;

public interface IStorySearchIndexRepository
{
    Task<IndexResponse> IndexAsync(Story entity, CancellationToken ct = default);
    Task<IndexResponse> IndexAsync(Story[] entities, CancellationToken ct = default);
    Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default);
}
