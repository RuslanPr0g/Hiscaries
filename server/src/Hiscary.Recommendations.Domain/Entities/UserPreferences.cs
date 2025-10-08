namespace Hiscary.Recommendations.Domain.Entities;

public sealed record UserPreferences
{
    public Guid Id { get; init; }
    public string[] FavoriteGenres { get; init; }
    public string[] FavoriteTags { get; init; }

    public static UserPreferences Create(Guid id)
    {
        return new UserPreferences
        {
            Id = id,
            FavoriteGenres = [],
            FavoriteTags = [],
        };
    }

    public UserPreferences LikeNew(IEnumerable<string> genres, IEnumerable<string> tags)
    {
        return LikeNewGenres(genres).LikeNewTags(tags);
    }

    public UserPreferences LikeNewGenres(IEnumerable<string> genres)
    {
        var recent = genres.Distinct().ToArray();
        var newFavoriteGenres = FavoriteGenres.Except(recent).Concat(recent).TakeLast(7).ToArray();

        return new UserPreferences
        {
            Id = Id,
            FavoriteGenres = newFavoriteGenres,
            FavoriteTags = FavoriteTags
        };
    }

    public UserPreferences LikeNewTags(IEnumerable<string> tags)
    {
        var recent = tags.Distinct().ToArray();
        var newFavoriteTags = FavoriteTags.Except(recent).Concat(recent).TakeLast(15).ToArray();

        return new UserPreferences
        {
            Id = Id,
            FavoriteGenres = FavoriteGenres,
            FavoriteTags = newFavoriteTags
        };
    }
}
