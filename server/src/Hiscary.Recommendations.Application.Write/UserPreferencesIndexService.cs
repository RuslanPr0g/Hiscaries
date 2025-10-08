using Elastic.Clients.Elasticsearch;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Domain.Persistence.Write;
using Hiscary.Recommendations.Domain.Services.Write;

namespace Hiscary.Recommendations.Application.Write;

internal sealed class UserPreferencesIndexService : IUserPreferencesIndexService
{
    private readonly IStorySearchRepository _storySearchRepository;
    private readonly IUserPreferencesReadRepository _userPreferencesReadRepository;
    private readonly IUserPreferencesIndexRepository _userPreferencesIndexRepository;

    public UserPreferencesIndexService(
        IUserPreferencesIndexRepository userPreferencesIndexRepository,
        IUserPreferencesReadRepository userPreferencesReadRepository,
        IStorySearchRepository storySearchRepository)
    {
        _userPreferencesIndexRepository = userPreferencesIndexRepository;
        _userPreferencesReadRepository = userPreferencesReadRepository;
        _storySearchRepository = storySearchRepository;
    }

    public async Task<IndexResponse?> AddOrUpdateAsync(Guid userAccountId, Guid storyId, CancellationToken ct = default)
    {
        var user = await _userPreferencesReadRepository.GetByIdAsync(userAccountId, ct);

        if (user is null)
        {
            user = UserPreferences.Create(userAccountId);
        }

        var story = await _storySearchRepository.GetByIdAsync(storyId, ct);

        if (story is null)
        {
            return null;
        }

        var genres = story.Genres;
        var tags = story.Title.Split(' ').Union(story.Description.Split(' '));

        user = user.LikeNew(genres, tags);

        return await _userPreferencesIndexRepository.IndexAsync(user, ct);
    }

    public async Task<DeleteResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _userPreferencesIndexRepository.DeleteAsync(id, ct);
    }
}
