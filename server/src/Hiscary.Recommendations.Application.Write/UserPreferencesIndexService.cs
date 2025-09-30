using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Services.Write;

namespace Hiscary.Recommendations.Application.Write;

internal sealed class UserPreferencesIndexService : IUserPreferencesIndexService
{
    public Task AddOrUpdateAsync(UserPreferences entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
