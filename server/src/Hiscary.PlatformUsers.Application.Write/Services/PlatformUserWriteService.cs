using Hiscary.Media.IntegrationEvents.Incoming;
using Hiscary.PlatformUsers.Domain;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.Domain.Services;
using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.Constants;
using Hiscary.Shared.Domain.Extensions;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventPublishers;
using StackNucleus.DDD.Domain.Generators;
using StackNucleus.DDD.Domain.ResultModels;

namespace Hiscary.PlatformUsers.Application.Write.Services;

public sealed class PlatformUserWriteService(
    IEventPublisher publisher,
    IPlatformUserWriteRepository repository,
    IIdGenerator idGenerator,
    ILogger<PlatformUserWriteService> logger) : IPlatformUserWriteService
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IPlatformUserWriteRepository _repository = repository;
    private readonly IIdGenerator _idGenerator = idGenerator;
    private readonly ILogger<PlatformUserWriteService> _logger = logger;

    public async Task<OperationResult> BecomePublisher(Guid userAccountId)
    {
        _logger.LogInformation("Attempting to make user {id} a publisher", userAccountId);
        PlatformUser? user = await _repository.GetByUserAccountId(userAccountId);

        if (user is null)
        {
            _logger.LogWarning("Failed to make user a publisher: User account {id} not found", userAccountId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        var libraryId = _idGenerator.Generate((id) => new LibraryId(id));
        user.BecomePublisher(libraryId);

        // TODO: integration events should have outbox too
        await _publisher.Publish(
            new UserBecamePublisherIntegrationEvent(
                user.Id, user.UserAccountId));

        await _repository.SaveChanges();

        _logger.LogInformation("Successfully made user {id} a publisher", userAccountId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> PublishReview(Guid libraryId, Guid reviewerId, string? message, Guid? reviewId)
    {
        _logger.LogInformation("Attempting to publish review for reviewer {ReviewerId}, publisher {LibraryId}", reviewerId, libraryId);
        PlatformUser? user = await _repository.GetByUserAccountId(reviewerId);

        if (user is null)
        {
            _logger.LogWarning("Failed to publish review: User {ReviewerId} not found", reviewerId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        user.PublishReview(libraryId, message);
        await _repository.SaveChanges();

        _logger.LogInformation("Successfully published review for reviewer {ReviewerId}, publisher {LibraryId}", reviewerId, libraryId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> DeleteReview(Guid userId, Guid libraryId)
    {
        _logger.LogInformation("Attempting to delete review {LibraryId} for user {UserId}", libraryId, userId);
        PlatformUser? user = await _repository.GetByUserAccountId(userId);

        if (user is null)
        {
            _logger.LogWarning("Failed to delete review: User {UserId} not found", userId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        user.RemoveReview(libraryId);
        await _repository.SaveChanges();

        _logger.LogInformation("Successfully deleted review {LibraryId} for user {UserId}", libraryId, userId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> BookmarkStory(Guid userId, Guid storyId)
    {
        _logger.LogInformation("Attempting to bookmark story for user {UserId}", userId);
        PlatformUser? user = await _repository.GetByUserAccountId(userId);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found when attempting to bookmark story", userId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        user.BookmarkStory(storyId);
        await _repository.SaveChanges();

        _logger.LogInformation("Successfully bookmarked story {StoryId} for user {UserId}", storyId, userId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> ReadStoryPage(Guid userId, Guid storyId, int page)
    {
        _logger.LogInformation("Attempting to record read story history for user {UserId}, story {StoryId}, page {Page}", userId, storyId, page);
        PlatformUser? user = await _repository.GetByUserAccountId(userId);

        if (user is null)
        {
            _logger.LogWarning("Failed to record read story history: User {UserId} not found", userId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        user.ReadStoryPage(storyId, page);
        await _repository.SaveChanges();

        _logger.LogInformation("Successfully recorded read story history for user {UserId}, story {StoryId}, page {Page}", userId, storyId, page);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> EditLibraryInfo(
        Guid userId,
        Guid libraryId,
        string? bio,
        byte[]? avatar,
        bool shouldUpdateImage,
        List<string> linksToSocialMedia)
    {
        _logger.LogInformation("Attempting to edit library for user {UserId}", userId);
        PlatformUser? user = await _repository.GetByUserAccountId(userId);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found when attempting to edit a library", userId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        if (!user.Libraries.Any(x => x.Id.Value == libraryId))
        {
            _logger.LogWarning("User {UserId} is not an owner of the library {LibraryId}", userId, libraryId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.NoRights);
        }

        user.EditLibrary(libraryId, bio, linksToSocialMedia);

        var avatarIsEmpty = avatar is null || avatar.Length <= 0;

        if (shouldUpdateImage && avatarIsEmpty)
        {
            user.ClearAvatarUrl(libraryId);
        }

        if (shouldUpdateImage && !avatarIsEmpty && avatar is not null)
        {
            await _publisher.Publish(
                new ImageUploadRequestedIntegrationEvent(
                    avatar, libraryId, "users", ImageSizeExtensions.AllImageSizes()));
        }

        await _repository.SaveChanges();

        _logger.LogInformation("Successfully edited a library {LibraryId} for user {UserId}", libraryId, userId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> SubscribeToLibrary(Guid userId, Guid libraryId)
    {
        _logger.LogInformation("Attempting to subscribe to a library {LibraryId} for a user {UserId}", libraryId, userId);
        PlatformUser? user = await _repository.GetByUserAccountId(userId);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found when attempting to subscribe to a library", userId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        if (user.Libraries.Any(x => x.Id.Value == libraryId))
        {
            _logger.LogWarning("User {UserId} is an owner of the library, so the subscription is not possible {LibraryId}", userId, libraryId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.NoRights);
        }

        user.SubscribeToLibrary(libraryId);
        await _repository.SaveChanges();

        _logger.LogInformation("Successfully subscribed to the library {LibraryId} for the user {UserId}", libraryId, userId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> UnsubscribeFromLibrary(Guid userId, Guid libraryId)
    {
        _logger.LogInformation("Attempting to unsubscribe from a library {LibraryId} for a user {UserId}", libraryId, userId);
        PlatformUser? user = await _repository.GetByUserAccountId(userId);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found when attempting to unsubscribe from a library", userId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        }

        if (!user.Subscriptions.Any(x => x.LibraryId.Value == libraryId))
        {
            _logger.LogWarning("User {UserId} is not subscribed to the library {LibraryId}", userId, libraryId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.NoRights);
        }

        user.UnsubscribeFromLibrary(libraryId);
        await _repository.SaveChanges();

        _logger.LogInformation("Successfully unsubscribed from the library {LibraryId} for the user {UserId}", libraryId, userId);
        return OperationResult.CreateSuccess();
    }
}