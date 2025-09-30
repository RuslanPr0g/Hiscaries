using Hiscary.Recommendations.Domain.Entities;

namespace Hiscary.Recommendations.Domain.Persistence.Write;

public interface IUserPreferencesIndexRepository
{
    Task IndexAsync(UserPreferences entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
