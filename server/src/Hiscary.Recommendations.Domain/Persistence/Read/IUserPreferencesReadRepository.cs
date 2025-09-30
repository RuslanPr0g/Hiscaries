using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Persistence.Read;

public interface IUserPreferencesReadRepository
{
    Task<UserPreferences?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
