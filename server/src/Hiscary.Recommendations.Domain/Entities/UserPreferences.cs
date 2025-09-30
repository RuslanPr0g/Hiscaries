namespace Hiscary.Recommendations.Domain.Entities;

public sealed record UserPreferences
{
    /// <summary>
    /// This is a user account id
    /// </summary>
    public Guid Id { get; init; }

    public HashSet<string> FavoriteGenres { get; private set; }

    public HashSet<string> FavoriteTags { get; private set; }

    public void LikeNewGenres(HashSet<string> genres)
    {
        FavoriteGenres.UnionWith(genres);
    }

    public void LikeNewTags(HashSet<string> tags)
    {
        FavoriteTags.UnionWith(tags);
    }
}
