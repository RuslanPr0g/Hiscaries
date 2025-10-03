namespace Hiscary.Recommendations.Domain.Persistence.Read;

public interface ISystemDataAvailabilityRepository
{
    Task<bool> IsUserDataAvailable(CancellationToken ct = default);
    Task<bool> IsStoryDataAvailable(CancellationToken ct = default);
}
