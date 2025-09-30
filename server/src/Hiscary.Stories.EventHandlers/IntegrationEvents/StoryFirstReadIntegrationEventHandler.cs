using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.Stories.EventHandlers.IntegrationEvents;

public sealed class StoryFirstReadIntegrationEventHandler(
    IEventPublisher publisher,
    IStoryWriteRepository repository,
    ILogger<StoryFirstReadIntegrationEventHandler> logger)
        : IEventHandler<StoryFirstReadIntegrationEvent>
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IStoryWriteRepository _repository = repository;

    public async Task Handle(
        StoryFirstReadIntegrationEvent integrationEvent, IMessageContext context)
    {
        var storyId = integrationEvent.StoryId;

        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            return;
        }

        story.ReadStoryUniquely();

        await _publisher.Publish(
            new StoryUniqueReadCountIncreasedIntegrationEvent(storyId, integrationEvent.UserAccountId, story.UniqueReads));

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(StoryFirstReadIntegrationEventHandler));
    }
}
