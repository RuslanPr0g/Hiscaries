using Hiscary.PlatformUsers.Domain.ProcessModels;
using Hiscary.PlatformUsers.Domain.ReadModels;

namespace Hiscary.PlatformUsers.Domain.Services;

public interface IPlatformUserReadService
{
    Task<PlatformUserReadModel?> GetUserById(Guid userAccountId);
    Task<LibraryReadModel?> GetLibraryInformation(Guid requesterId, LibraryId? libraryId = null);

    Task<IEnumerable<UserReadingStoryMetadataReadModel>> GetUserReadingStoryMetadata(
        Guid requesterUserAccountId,
        UserReadingStoryProcessModel[] stories);
    Task<IEnumerable<Guid>> GetResumeReadingStoryIds(Guid userAccountId);
    Task<IEnumerable<LastReadAtDateToId>> GetReadingHistoryStoryIds(Guid userAccountId);
}