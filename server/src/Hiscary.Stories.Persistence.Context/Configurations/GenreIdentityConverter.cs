using Hiscary.Stories.Domain.Genres;
using StackNucleus.DDD.Persistence.EF.Postgres.Configurations;

namespace Hiscary.Stories.Persistence.Context.Configurations;

public class GenreIdentityConverter : IdentityConverter<GenreId>
{
    public GenreIdentityConverter() :
        base((x) => new GenreId(x))
    {
    }
}
