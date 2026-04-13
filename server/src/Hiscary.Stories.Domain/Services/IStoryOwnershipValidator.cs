public interface IStoryOwnershipValidator
{
    Task<bool> IsOwnerOrAdmin(Guid storyId, Guid callerId, string callerRole);
    Task<bool> IsCommentOwner(Guid storyId, Guid commentId, Guid callerId, string callerRole);
}
