using Hiscary.Stories.Domain.Genres;
using StackNucleus.DDD.Domain.Repositories;

namespace Hiscary.Stories.Domain.DataAccess;

public interface IGenreWriteRepository : IBaseWriteRepository<Genre, GenreId>
{
}