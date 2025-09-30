using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Persistence.Write;

public interface IStorySearchIndexRepository
{
    Task IndexAsync(Story entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
