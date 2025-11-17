using Hiscary.PlatformUsers.Domain;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using StackNucleus.DDD.Persistence.EF.Postgres;

namespace Hiscary.PlatformUsers.Persistence.Write;

public class PlatformUserWriteRepository(PlatformUsersContext context) :
    BaseWriteRepository<PlatformUser, PlatformUserId, PlatformUsersContext>,
    IPlatformUserWriteRepository
{
    protected override PlatformUsersContext Context { get; init; } = context;

    public async Task<List<Guid>> GetLibrarySubscribersUserAccountIds(LibraryId libraryId) =>
        await Context.PlatformUsers
        .Include(x => x.Subscriptions)
        .Where(x => x.Subscriptions.Any(y => y.LibraryId == libraryId))
        .Select(x => x.UserAccountId)
        .ToListAsync();

    public async Task<PlatformUser?> GetLibraryOwnerByLibraryId(LibraryId libraryId) =>
        await Context.PlatformUsers
        .Include(x => x.ReadHistory)
        .Include(x => x.Libraries)
        .Include(x => x.Bookmarks)
        .Include(x => x.Reviews)
        .Include(x => x.Subscriptions)
        .FirstOrDefaultAsync(x => x.Libraries.Any(x => x.Id == libraryId));

    public async Task<PlatformUser?> GetByUserAccountId(Guid userAccountId) =>
        await Context.PlatformUsers
        .Include(x => x.ReadHistory)
        .Include(x => x.Libraries)
        .Include(x => x.Bookmarks)
        .Include(x => x.Reviews)
        .Include(x => x.Subscriptions)
        .FirstOrDefaultAsync(x => x.UserAccountId == userAccountId);

    public async Task<List<Guid>> GetStoryReaderUserIdsByStoryId(Guid storyId) =>
        await Context.PlatformUsers
        .Include(x => x.ReadHistory)
        .Where(x => x.ReadHistory.Any(y => y.StoryId == storyId))
        .Select(x => x.UserAccountId)
        .ToListAsync();
}
