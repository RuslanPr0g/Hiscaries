namespace Hiscary.Recommendations.Domain.Entities;

public sealed record UserPreferences
{
    /// <summary>
    /// This is a user account id
    /// </summary>
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
        var newFavoriteGenres = FavoriteGenres.ToHashSet();
        newFavoriteGenres.UnionWith(genres);
        
        return new UserPreferences
        {
            Id = Id,
            FavoriteGenres = newFavoriteGenres.ToArray(),
            FavoriteTags = FavoriteTags
        };
    }

    public UserPreferences LikeNewTags(IEnumerable<string> tags)
    {
        var newFavoriteTags = FavoriteTags.ToHashSet();
        newFavoriteTags.UnionWith(tags);
               
        return new UserPreferences
        {
            Id = Id,
            FavoriteGenres = FavoriteGenres,
            FavoriteTags = newFavoriteTags.ToArray()
        };
    }
}
