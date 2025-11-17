using StackNucleus.DDD.Domain.Repositories;

namespace Hiscary.PlatformUsers.Domain.DataAccess;

public interface IPlatformUserWriteRepository : IBaseWriteRepository<PlatformUser, PlatformUserId>
{
    Task<PlatformUser?> GetLibraryOwnerByLibraryId(LibraryId libraryId);
    Task<PlatformUser?> GetByUserAccountId(Guid userAccountId);
    Task<List<Guid>> GetLibrarySubscribersUserAccountIds(LibraryId libraryId);
    Task<List<Guid>> GetStoryReaderUserIdsByStoryId(Guid storyId);
}
