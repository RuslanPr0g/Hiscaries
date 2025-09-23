using StackNucleus.DDD.Domain.Repositories;
using Hiscary.PlatformUsers.Domain.ProcessModels;
using Hiscary.PlatformUsers.Domain.ReadModels;

namespace Hiscary.PlatformUsers.Domain.DataAccess;

public interface IPlatformUserReadRepository : IBaseReadRepository<PlatformUserReadModel>
{
    Task<PlatformUserReadModel?> GetUserById(PlatformUserId userId);
    Task<PlatformUserReadModel?> GetPlatformUserByAccountUserId(Guid userAccountId);
    Task<PlatformUserId?> GetPlatformUserIdByUserAccountId(Guid userAccountId);
    Task<LibraryReadModel?> GetLibraryInformation(Guid requesterUserAccountId, LibraryId? libraryId);
    Task<IEnumerable<UserReadingStoryMetadataReadModel>> GetUserReadingStoryMetadataInformation(
        Guid requesterUserAccountId,
        UserReadingStoryProcessModel[] stories);
    Task<IEnumerable<Guid>> GetResumeReadingStoryIds(Guid userAccountId);
    Task<IEnumerable<LastReadAtDateToId>> GetReadingHistoryStoryIds(Guid userAccountId);
}