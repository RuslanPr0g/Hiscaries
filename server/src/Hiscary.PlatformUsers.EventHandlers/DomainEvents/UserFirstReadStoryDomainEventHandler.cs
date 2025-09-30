using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.DomainEvents;

public sealed class UserFirstReadStoryDomainEventHandler(
    IEventPublisher publisher,
    ILogger<UserFirstReadStoryDomainEventHandler> logger)
        : IEventHandler<UserFirstReadStoryDomainEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(
        UserFirstReadStoryDomainEvent domainEvent, IMessageContext context)
    {
        await _publisher.Publish(new StoryFirstReadIntegrationEvent(domainEvent.UserAccountId, domainEvent.StoryId));

        logger.LogInformation("{Handler} handled.", nameof(UserFirstReadStoryDomainEventHandler));
    }
}
