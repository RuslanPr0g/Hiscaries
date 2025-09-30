using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Services.Write;

public interface IStorySearchIndexService
{
    Task AddOrUpdateAsync(Story entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
