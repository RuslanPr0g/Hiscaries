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

    public async Task AddOrUpdateAsync(Guid userAccountId, Guid storyId, CancellationToken ct = default)
    {
        var user = await _userPreferencesReadRepository.GetByIdAsync(userAccountId, ct);

        if (user is null)
        {
            user = new UserPreferences
            {
                Id = userAccountId
            };
        }

        var story = await _storySearchRepository.GetByIdAsync(storyId, ct);

        if (story is null)
        {
            return;
        }

        var genres = story.Genres.ToHashSet();
        var tags = story.Title.Split(' ').Union(story.Description.Split(' ')).ToHashSet();

        user.LikeNewGenres(genres);
        user.LikeNewTags(tags);

        await _userPreferencesIndexRepository.IndexAsync(user, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _userPreferencesIndexRepository.DeleteAsync(id, ct);
    }
}
