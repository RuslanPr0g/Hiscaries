using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class StoryContentsChangedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    IEventPublisher publisher,
    ILogger<StoryContentsChangedIntegrationEventHandler> logger)
        : IEventHandler<StoryContentsChangedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _platformUserRepository = repository;
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(
        StoryContentsChangedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var userIds = await _platformUserRepository.GetStoryReaderUserIdsByStoryId(integrationEvent.StoryId);

        foreach (var userId in userIds)
        {
            var integrationEventForHistoryUpdate = new UserReadStoryChangedContentsDomainEvent(
                userId,
                integrationEvent.StoryId,
                integrationEvent.NumberOfPages);

            await _publisher.Publish(integrationEventForHistoryUpdate);
        }

        logger.LogInformation("{Handler} handled.", nameof(StoryContentsChangedIntegrationEventHandler));
    }
}
