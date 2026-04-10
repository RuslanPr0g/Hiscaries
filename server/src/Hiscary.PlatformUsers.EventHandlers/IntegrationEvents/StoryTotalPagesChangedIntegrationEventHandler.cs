using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class StoryTotalPagesChangedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    IEventPublisher publisher,
    ILogger<StoryTotalPagesChangedIntegrationEventHandler> logger)
        : IEventHandler<StoryTotalPagesChangedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _platformUserRepository = repository;
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(
        StoryTotalPagesChangedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var userIds = await _platformUserRepository.GetStoryReaderUserIdsByStoryId(integrationEvent.StoryId);

        foreach (var userId in userIds)
        {
            var integrationEventForHistoryUpdate = new UserReadStoryChangedContentsDomainEvent(
                userId,
                integrationEvent.StoryId,
                integrationEvent.NewTotalPages);

            await _publisher.Publish(integrationEventForHistoryUpdate);
        }

        logger.LogInformation("{Handler} handled.", nameof(StoryTotalPagesChangedIntegrationEventHandler));
    }
}
