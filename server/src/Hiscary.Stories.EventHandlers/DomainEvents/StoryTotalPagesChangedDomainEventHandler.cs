using Hiscary.Stories.DomainEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.Stories.EventHandlers.DomainEvents;

public sealed class StoryTotalPagesChangedDomainEventHandler(
    IEventPublisher publisher,
    ILogger<StoryTotalPagesChangedDomainEventHandler> logger)
        : IEventHandler<StoryTotalPagesChangedDomainEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(
        StoryTotalPagesChangedDomainEvent domainEvent, IMessageContext context)
    {
        await _publisher.Publish(new StoryTotalPagesChangedIntegrationEvent(domainEvent.StoryId, domainEvent.NewTotalPages));

        logger.LogInformation("{Handler} handled.", nameof(StoryTotalPagesChangedDomainEventHandler));
    }
}
