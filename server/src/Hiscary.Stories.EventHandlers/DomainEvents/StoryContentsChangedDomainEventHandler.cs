using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.DomainEvents;

public sealed class StoryContentsChangedDomainEventHandler(
    IEventPublisher publisher,
    ILogger<StoryContentsChangedDomainEventHandler> logger)
        : IEventHandler<StoryContentsChangedDomainEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(
        StoryContentsChangedDomainEvent domainEvent, IMessageContext context)
    {
        await _publisher.Publish(new StoryContentsChangedIntegrationEvent(domainEvent.StoryId, domainEvent.NumberOfPages));

        logger.LogInformation("{Handler} handled.", nameof(StoryContentsChangedDomainEventHandler));
    }
}
