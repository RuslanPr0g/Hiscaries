using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Hiscary.Stories.Persistence.Write;

public class LibraryOwnerRepository(StoriesContext context) : ILibraryOwnerRepository
{
    private readonly StoriesContext _context = context;

    public async Task<Guid?> GetOwnerUserAccountIdByLibraryId(Guid libraryId)
    {
        return await _context.Stories
            .Where(story => story.LibraryId == libraryId)
            .FirstOrDefaultAsync();
    }
}
