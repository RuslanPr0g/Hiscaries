using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.Stories.Domain.DataAccess;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Stories.EventHandlers.IntegrationEvents;

public sealed class StoryFirstReadIntegrationEventHandler(
    IStoryWriteRepository repository,
    ILogger<StoryFirstReadIntegrationEventHandler> logger)
        : IEventHandler<StoryFirstReadIntegrationEvent>
{
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

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(StoryFirstReadIntegrationEventHandler));
    }
}
