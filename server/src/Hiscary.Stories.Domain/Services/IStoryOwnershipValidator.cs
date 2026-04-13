public interface IStoryOwnershipValidator
{
    Task<bool> IsOwnerOrAdmin(Guid storyId, Guid callerLibraryId, string callerRole);
    Task<bool> IsCommentOwner(Guid storyId, Guid commentId, Guid callerPlatformUserId, string callerRole);
}
