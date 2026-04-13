namespace Hiscary.Stories.Domain.DataAccess;

public interface ILibraryOwnerRepository
{
    /// <summary>
    /// Returns the UserAccountId (JWT "id" claim) of the owner of the given library,
    /// or null if the library does not exist.
    /// </summary>
    Task<Guid?> GetOwnerUserAccountIdByLibraryId(Guid libraryId);
}
