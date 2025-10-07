using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Services.Write;

public interface IStorySearchIndexService
{
    Task<IndexResponse> AddOrUpdateAsync(Story entity, CancellationToken ct = default);
    Task<BulkResponse> AddOrUpdateAsync(Story[] entities, CancellationToken ct = default);
    Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default);
}
