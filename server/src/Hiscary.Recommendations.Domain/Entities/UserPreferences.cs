namespace Hiscary.Recommendations.Domain.Entities;

public sealed class UserPreferences
{
    /// <summary>
    /// This is a user account id
    /// </summary>
    public Guid Id { get; set; }

    public HashSet<string> FavoriteGenres { get; set; }

    public HashSet<string> FavoriteTags { get; set; }
}
