using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.Domain.ProcessModels;
using Hiscary.PlatformUsers.Domain.ReadModels;
using Hiscary.PlatformUsers.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using StackNucleus.DDD.Domain.Extensions;
using StackNucleus.DDD.Persistence.EF.Postgres;

namespace Hiscary.PlatformUsers.Persistence.Read;

public class PlatformUserReadRepository(PlatformUsersContext context) :
    BaseReadRepository<PlatformUserReadModel>,
    IPlatformUserReadRepository
{
    private PlatformUsersContext Context { get; init; } = context;


    public async Task<PlatformUserReadModel?> GetUserById(PlatformUserId userId) =>
        await Context.PlatformUsers
            .AsNoTracking()
            .Where(x => x.UserAccountId == userId.Value)
            .Select(user => PlatformUserReadModel.FromDomainModel(user))
            .FirstOrDefaultAsync();

    public async Task<PlatformUserReadModel?> GetPlatformUserByAccountUserId(Guid userAccountId) =>
        await Context.PlatformUsers
            .AsNoTracking()
            .Where(x => x.UserAccountId == userAccountId)
            .Select(user => PlatformUserReadModel.FromDomainModel(user))
            .FirstOrDefaultAsync();

    public async Task<PlatformUserId?> GetPlatformUserIdByUserAccountId(Guid userAccountId) =>
        await Context.PlatformUsers
            .AsNoTracking()
            .Where(x => x.UserAccountId == userAccountId)
            .Select(user => user.Id)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Guid>> GetResumeReadingStoryIds(Guid userAccountId) =>
        await Context.PlatformUsers
            .AsNoTracking()
            .Where(_ => _.UserAccountId == userAccountId)
            .Include(_ => _.ReadHistory)
            .SelectMany(_ => _.ReadHistory)
            .OrderByDescending(_ => _.EditedAt)
            .Take(3)
            .Select(_ => _.StoryId)
            .ToListAsync();

    public async Task<IEnumerable<LastReadAtDateToId>> GetReadingHistoryStoryIds(Guid userAccountId) =>
        await Context.PlatformUsers
            .AsNoTracking()
            .Where(_ => _.UserAccountId == userAccountId)
            .Include(_ => _.ReadHistory)
            .SelectMany(_ => _.ReadHistory)
            .OrderBy(_ => _.EditedAt)
            .Select(_ => LastReadAtDateToId.Create(_.StoryId, _.EditedAt))
            .ToListAsync();

    public async Task<LibraryReadModel?> GetLibraryInformation(
        Guid requesterUserAccountId,
        LibraryId? libraryId)
    {
        var isUserSubscribedToLibrary = libraryId is not null && await IsUserSubscribedToLibrary(requesterUserAccountId, libraryId);

        if (libraryId is null)
        {
            return await Context.Libraries
                .AsNoTracking()
                .Include(x => x.PlatformUser)
                .Where(x => x.PlatformUser.UserAccountId == requesterUserAccountId)
                .Select(x => LibraryReadModel.FromDomainModel(x, requesterUserAccountId, isUserSubscribedToLibrary))
                .FirstOrDefaultAsync();
        }

        return await Context.Libraries
            .AsNoTracking()
            .Include(x => x.PlatformUser)
            .Where(x => x.Id == libraryId)
            .Select(x => LibraryReadModel.FromDomainModel(x, requesterUserAccountId, isUserSubscribedToLibrary))
            .FirstOrDefaultAsync();
    }

    private async Task<bool> IsUserSubscribedToLibrary(Guid requesterId, LibraryId libraryId)
    {
        return await Context.PlatformUserToLibrarySubscriptions
            .AsNoTracking()
            .Include(x => x.PlatformUser)
            .AnyAsync(x => x.LibraryId == libraryId && x.PlatformUser.UserAccountId == requesterId);
    }

    public async Task<IEnumerable<UserReadingStoryMetadataReadModel>> GetUserReadingStoryMetadataInformation(
        Guid requesterUserAccountId,
        UserReadingStoryProcessModel[] stories)
    {
        var currentUser = await Context.PlatformUsers
            .AsNoTracking()
            .AsSplitQuery()
            .Include(_ => _.Libraries)
            .Include(_ => _.ReadHistory)
            .FirstOrDefaultAsync(_ => _.UserAccountId == requesterUserAccountId);

        if (currentUser is null)
        {
            return [];
        }

        var libraryIds = stories.Select(s => s.LibraryId).Distinct().ToList();

        var libraryIdToNameDictionary = await Context.Libraries
            .Where(_ => libraryIds.Contains(_.Id))
            .Select(_ => new { LibraryId = _.Id, LibraryName = _.PlatformUser.Username })
            .ToDictionaryAsync(_ => _.LibraryId);

        var storyIdToLibraryNameDictionary = stories.Select(_ =>
        {
            var exists = libraryIdToNameDictionary.TryGetValue(_.LibraryId, out var lib);
            var libName = exists && lib is not null ? lib.LibraryName : null;

            return new
            {
                _.StoryId,
                LibraryName = exists && !string.IsNullOrWhiteSpace(libName) ? libName : null,
                Story = _
            };
        }).ToDictionary(_ => _.StoryId);

        var storyIdToReadHistory = currentUser.ReadHistory.ToDictionary(_ => _.StoryId);

        var storiesMetadata = stories.Select(_ =>
        {
            var exists = storyIdToLibraryNameDictionary.TryGetValue(_.StoryId, out var storyToLib);
            var libName = exists && storyToLib is not null ? storyToLib.LibraryName : null;
            var story = exists && storyToLib is not null ? storyToLib.Story : null;
            var canBeEdited = currentUser is null || story is null ?
                false :
                currentUser.Libraries.Any(_ => _.Id.Value == story.LibraryId);

            var readingHistoryExists = storyIdToReadHistory.TryGetValue(_.StoryId, out var readingHistory);
            var lastPageRead = readingHistory?.LastPageRead;

            return new UserReadingStoryMetadataReadModel
            {
                StoryId = _.StoryId,
                LibraryName = libName,
                IsEditable = canBeEdited,
                PercentageRead = RetrieveReadingProgressForAUser(
                lastPageRead,
                story?.TotalPages),
                LastPageRead = lastPageRead ?? 0
            };
        });

        return storiesMetadata;
    }

    private static decimal RetrieveReadingProgressForAUser(int? lastPageRead, int? totalPages)
    {
        if (!totalPages.HasValue || !lastPageRead.HasValue)
        {
            return 0;
        }

        int currentPage = lastPageRead.Value + 1;
        return currentPage.PercentageOf(totalPages.Value);
    }
}
