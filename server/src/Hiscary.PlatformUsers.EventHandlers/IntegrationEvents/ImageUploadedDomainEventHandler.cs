using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.Shared.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class ImageUploadedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<ImageUploadedIntegrationEventHandler> logger)
        : IEventHandler<ImageUploadedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _platformUserRepository = repository;

    public async Task Handle(
        ImageUploadedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var imageUrls = integrationEvent.ImageUrls;
        var libraryId = integrationEvent.RequesterId;

        if (imageUrls is null || imageUrls.Length == 0)
        {
            return;
        }

        var user = await _platformUserRepository.GetLibraryOwnerByLibraryId(libraryId);

        if (user is null)
        {
            return;
        }

        user.UpdateAvatarUrl(libraryId, ImageContainer.FromImageUrlToSize(imageUrls));

        await _platformUserRepository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(ImageUploadedIntegrationEventHandler));
    }
}
