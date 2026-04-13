namespace Hiscary.Media.Api.Rest.Authorization;

public interface IMediaOwnershipValidator
{
    Task<bool> IsStoryOwnerOrAdmin(Guid storyId, Guid callerId, string callerRole);
}
