using Hiscary.Notifications.Domain;
using Hiscary.Notifications.Domain.DataAccess;
using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.Generators;
using Wolverine;

namespace Hiscary.Notifications.EventHandlers.IntegrationEvents;

public sealed class UserPublishedStoryIntegrationEventHandler(
    INotificationWriteRepository repository,
    IIdGenerator idGenerator,
    ILogger<UserPublishedStoryIntegrationEventHandler> logger)
        : IEventHandler<UserPublishedStoryIntegrationEvent>
{
    private readonly INotificationWriteRepository _repository = repository;
    private readonly IIdGenerator _idGenerator = idGenerator;

    public async Task Handle(
        UserPublishedStoryIntegrationEvent integrationEvent, IMessageContext context)
    {
        var userIds = integrationEvent.SubscriberIds;

        var notifications = new List<Notification>();

        foreach (var userId in userIds)
        {
            var notificationId = _idGenerator.Generate((val) => new NotificationId(val));
            var notification = Notification.CreatePublishedNotification(
                notificationId,
                userId,
                integrationEvent.Title,
                // TODO: should be in consts
                "StoryPublished",
                integrationEvent.StoryId,
                ImageContainer.FromImageUrlToSize(integrationEvent.ImageUrls));
            notifications.Add(notification);
        }

        await _repository.AddRange(notifications.ToArray());

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(UserPublishedStoryIntegrationEventHandler));
    }
}
