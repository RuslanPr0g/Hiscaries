using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Hiscary.Stories.Persistence.Write;

public class LibraryOwnerRepository(StoriesContext context) : ILibraryOwnerRepository
{
    private readonly StoriesContext _context = context;

    public async Task<Guid?> GetOwnerUserAccountIdByLibraryId(Guid libraryId)
    {
        var result = await _context.Database
            .SqlQueryRaw<Guid>(
                """
                SELECT pu."UserAccountId"
                FROM platformusers."Libraries" l
                JOIN platformusers."PlatformUsers" pu ON pu."Id" = l."PlatformUserId"
                WHERE l."Id" = {0}
                """,
                libraryId)
            .FirstOrDefaultAsync();

        return result == Guid.Empty ? null : result;
    }
}
