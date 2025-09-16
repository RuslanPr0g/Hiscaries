using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class StoryPublishedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    IEventPublisher publisher,
    ILogger<StoryPublishedIntegrationEventHandler> logger)
        : IEventHandler<StoryPublishedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _platformUserRepository = repository;
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(
        StoryPublishedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var userIds = await _platformUserRepository.GetLibrarySubscribersUserAccountIds(integrationEvent.LibraryId);

        var integrationEventForNotification = new UserPublishedStoryIntegrationEvent(
            userIds.ToArray(),
            integrationEvent.LibraryId,
            integrationEvent.StoryId,
            integrationEvent.Title,
            integrationEvent.ImageUrls);

        await _publisher.Publish(integrationEventForNotification);

        logger.LogInformation("{Handler} handled.", nameof(StoryPublishedIntegrationEventHandler));
    }
}
