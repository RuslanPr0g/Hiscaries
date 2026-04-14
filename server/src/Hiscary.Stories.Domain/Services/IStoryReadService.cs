using Hiscary.Stories.Domain;
using Hiscary.Stories.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using StackNucleus.DDD.Domain.ClientModels;

public interface IStoryReadService
{
    Task<StoryWithContentsReadModel?> GetStoryById(Guid storyId);

    Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryByIds(
        StoryId[] storyIds,
        StoryClientQueryableModelWithSortableRules queryableModel);

    Task<IEnumerable<GenreReadModel>> GetAllGenres();

    Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryRecommendations(StoryClientQueryableModelWithSortableRules queryableModel);

    Task<ClientQueriedModel<StorySimpleReadModel>> SearchForStory(
        StoryClientQueryableModelWithSortableRules queryableModel,
        Guid? storyId = null,
        Guid? libraryId = null,
        string? searchTerm = null,
        string? genre = null);
}