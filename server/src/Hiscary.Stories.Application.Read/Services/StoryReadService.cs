using Hiscary.Stories.Domain;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Stories.Application.Read.Services;

public sealed class StoryReadService : IStoryReadService
{
    private readonly IStoryReadRepository _repository;

    public StoryReadService(IStoryReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GenreReadModel>> GetAllGenres() => await _repository.GetAllGenres();

    public async Task<StoryWithContentsReadModel?> GetStoryById(Guid storyId)
    {
        return await _repository.GetStory(storyId);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryByIds(
        StoryId[] storyIds,
        StoryClientQueryableModelWithSortableRules queryableModel)
    {
        return await _repository.GetStorySimpleInfo(storyIds, queryableModel);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> GetStoryRecommendations(StoryClientQueryableModelWithSortableRules queryableModel)
    {
        return await _repository.GetStoryReadingSuggestions(queryableModel);
    }

    public async Task<ClientQueriedModel<StorySimpleReadModel>> SearchForStory(
        StoryClientQueryableModelWithSortableRules queryableModel,
        Guid? storyId = null,
        Guid? libraryId = null,
        string? searchTerm = null,
        string? genre = null)
    {
        if (libraryId.HasValue && Guid.Empty != storyId)
        {
            return await _repository.GetStorySimpleInfoByLibraryId(libraryId.Value, queryableModel);
        }

        if (storyId.HasValue && Guid.Empty != storyId)
        {
            return await _repository.GetStorySimpleInfo([storyId.Value], queryableModel);
        }

        if (searchTerm is null)
        {
            return ClientQueriedModel<StorySimpleReadModel>.Empty;
        }

        return await _repository.GetStoriesBy(searchTerm, genre ?? string.Empty, queryableModel);
    }
}