using Hiscary.Stories.Domain;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using Hiscary.Stories.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using StackNucleus.DDD.Domain.ClientModels;
using StackNucleus.DDD.Persistence;
using StackNucleus.DDD.Persistence.EF.Postgres;
using StackNucleus.DDD.Persistence.Extensions;

namespace Hiscary.Stories.Persistence.Read;

public class StoryReadRepository(StoriesContext context) :
    BaseReadRepository<StorySimpleReadModel>,
    IStoryReadRepository
{
    private StoriesContext Context { get; init; } = context;

    public async Task<IEnumerable<GenreReadModel>> GetAllGenres() =>
        await Context.Genres.AsNoTracking()
            .Select(genre => GenreReadModel.FromDomainModel(genre))
            .ToListAsync();

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetAllStories(ClientQueryableModel query)
    {
        return await GetStoryReadModelsPaginatedBy(Context.Stories.AsNoTracking(), query);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetStoriesBy(
        string searchTerm,
        string genre,
        StoryClientQueryableModelWithSortableRules queryableModel)
    {
        var query = Context.Stories
            .AsNoTracking()
            .Include(_ => _.Genres)
            .Where(story =>
                (!string.IsNullOrWhiteSpace(genre) && story.Genres.Any(g => g.Name.Contains(genre))) ||
                EF.Functions.ILike(story.Title, $"%{searchTerm}%") ||
                EF.Functions.ILike(story.Description, $"%{searchTerm}%"));

        return await GetStoryReadModelsPaginatedBy(query, queryableModel);
    }

    public async Task<StoryWithContentsReadModel?> GetStory(StoryId storyId)
    {
        var story = await Context.Stories
            .AsNoTracking()
            .Include(x => x.Genres)
            .Include(x => x.Comments)
            .Include(x => x.Contents)
            .Where(story => story.Id == storyId)
            .FirstOrDefaultAsync();

        if (story is null)
        {
            return null;
        }

        return StoryDomainToReadDto(story);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetStorySimpleInfo(
        StoryId[] storyIds,
        StoryClientQueryableModelWithSortableRules queryableModel)
    {
        var query = Context.Stories
            .AsNoTracking()
            .Where(story => storyIds.Contains(story.Id));

        return await GetStoryReadModelsPaginatedBy(query, queryableModel);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryReadingSuggestions(StoryClientQueryableModelWithSortableRules queryableModel)
    {
        var query = Context.Stories
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Contents.Any());

        return await GetStoryReadModelsPaginatedBy(query, queryableModel);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetStorySimpleInfoByLibraryId(
        Guid libraryId,
        StoryClientQueryableModelWithSortableRules queryableModel)
    {
        var query = Context.Stories
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.LibraryId == libraryId)
            .OrderByDescending(_ => _.EditedAt);

        return await GetStoryReadModelsPaginatedBy(query, queryableModel);
    }

    private static async Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryReadModelsPaginatedBy(
        IQueryable<Story> query,
        ClientQueryableModel queryableModel)
    {
        var stories = await query.ApplyPagination(queryableModel).ToListAsync();
        var storyReadModels = stories.SelectSkipNulls(StoryDomainToSimpleReadDto);

        return ClientQueriedModel<StorySimpleReadModel>.Create(
            storyReadModels,
            await query.CountAsync(),
            stories.Count());
    }

    private static StorySimpleReadModel? StoryDomainToSimpleReadDto(Story? story)
    {
        if (story is null)
        {
            return null;
        }

        return StorySimpleReadModel.FromDomainModel(story);
    }

    private static StoryWithContentsReadModel? StoryDomainToReadDto(Story? story)
    {
        if (story is null)
        {
            return null;
        }

        return StoryWithContentsReadModel.FromDomainModel(story);
    }
}
