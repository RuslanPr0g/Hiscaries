using Hiscary.Stories.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using StackNucleus.DDD.Domain.ClientModels;
using StackNucleus.DDD.Domain.Repositories;

namespace Hiscary.Stories.Domain.DataAccess;

public interface IStoryReadRepository : IBaseReadRepository<StorySimpleReadModel>
{
    Task<IEnumerable<GenreReadModel>> GetAllGenres();

    Task<StoryWithContentsReadModel?> GetStory(StoryId storyId);

    Task<ClientQueriedModel<StorySimpleReadModel>> GetAllStories(ClientQueryableModel query);

    Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryReadingSuggestions(
        StoryClientQueryableModelWithSortableRules queryableModel);

    Task<ClientQueriedModel<StorySimpleReadModel>> GetStoriesBy(
        string searchTerm,
        string genre,
        StoryClientQueryableModelWithSortableRules queryableModel);

    Task<ClientQueriedModel<StorySimpleReadModel>> GetStorySimpleInfo(
        StoryId[] storyIds,
        StoryClientQueryableModelWithSortableRules queryableModel);

    Task<ClientQueriedModel<StorySimpleReadModel>> GetStorySimpleInfoByLibraryId(
        Guid libraryId,
        StoryClientQueryableModelWithSortableRules queryableModel);
}