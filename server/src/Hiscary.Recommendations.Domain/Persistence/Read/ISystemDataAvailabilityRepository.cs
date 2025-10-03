namespace Hiscary.Recommendations.Domain.Persistence.Read;

public interface ISystemDataAvailabilityRepository
{
    Task<bool> IsUserDataAvailable(CancellationToken ct = default);
    Task<bool> IsStoryDataAvailable(CancellationToken ct = default);
    Task CreateUserIndex(CancellationToken ct = default);
    Task CreateStoryIndex(CancellationToken ct = default);
}
