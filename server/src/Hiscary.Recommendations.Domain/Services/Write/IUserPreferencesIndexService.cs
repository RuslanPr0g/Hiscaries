namespace Hiscary.Recommendations.Domain.Services.Write;

public interface IUserPreferencesIndexService
{
    Task AddOrUpdateAsync(Guid userAccountId, Guid storyId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}