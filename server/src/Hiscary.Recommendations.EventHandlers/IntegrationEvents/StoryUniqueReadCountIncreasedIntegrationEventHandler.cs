using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Services.Write;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Recommendations.EventHandlers.IntegrationEvents;

public sealed class StoryUniqueReadCountIncreasedIntegrationEventHandler(
    IStorySearchIndexService storyService,
    ILogger<StoryUniqueReadCountIncreasedIntegrationEventHandler> logger)
        : IEventHandler<StoryUniqueReadCountIncreasedIntegrationEvent>
{
    private readonly IStorySearchIndexService _storyService = storyService;

    public async Task Handle(
        StoryUniqueReadCountIncreasedIntegrationEvent integrationEvent, IMessageContext context)
    {
        await _storyService.AddOrUpdateAsync(new Story
        {
            Id = integrationEvent.StoryId,
            UniqueReads = integrationEvent.UniqueReads,
        });

        logger.LogInformation("{Handler} handled.", nameof(StoryUniqueReadCountIncreasedIntegrationEventHandler));
    }
}
