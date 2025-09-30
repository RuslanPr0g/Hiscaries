namespace Hiscary.Recommendations.Domain.Entities;

public sealed class UserPreferences
{
    public Guid UserAccountId { get; set; }

    public HashSet<string> FavoriteGenres { get; set; }

    public HashSet<string> FavoriteTags { get; set; }
}
