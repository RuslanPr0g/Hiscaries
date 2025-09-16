using Hiscary.Notifications.Domain.DataAccess;
using Hiscary.Notifications.IntegrationEvents.Incoming;
using Hiscary.Shared.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Notifications.EventHandlers.IntegrationEvents;

public sealed class NotificationReferenceObjectIdPreviewChangedIntegrationEventHandler(
    INotificationWriteRepository repository,
    ILogger<NotificationReferenceObjectIdPreviewChangedIntegrationEventHandler> logger)
        : IEventHandler<NotificationReferenceObjectIdPreviewChangedIntegrationEvent>
{
    private readonly INotificationWriteRepository _repository = repository;

    public async Task Handle(
        NotificationReferenceObjectIdPreviewChangedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var notification = await _repository.GetByObjectReferenceId(integrationEvent.ObjectReferenceId);

        if (notification is null)
        {
            return;
        }

        notification.UpdateImageUrls(ImageContainer.FromImageUrlToSize(integrationEvent.ImageUrls));

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(NotificationReferenceObjectIdPreviewChangedIntegrationEventHandler));
    }
}
