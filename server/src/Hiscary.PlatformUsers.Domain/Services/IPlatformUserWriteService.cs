using StackNucleus.DDD.Domain.ResultModels;

namespace Hiscary.PlatformUsers.Domain.Services;

public interface IPlatformUserWriteService
{
    Task<OperationResult> BecomePublisher(Guid id);

    Task<OperationResult> PublishReview(
        Guid libraryId,
        Guid reviewerId,
        string? message,
        Guid? reviewId);

    Task<OperationResult> DeleteReview(Guid userId, Guid libraryId);

    Task<OperationResult> BookmarkStory(Guid userId, Guid storyId);
    Task<OperationResult> ReadStoryPage(Guid userId, Guid storyId, int page);

    Task<OperationResult> EditLibraryInfo(
        Guid userId,
        Guid libraryId,
        string? bio,
        byte[]? avatar,
        bool shouldUpdateImage,
        List<string> linksToSocialMedia);

    Task<OperationResult> SubscribeToLibrary(Guid userId, Guid libraryId);
    Task<OperationResult> UnsubscribeFromLibrary(Guid userId, Guid libraryId);
}