using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Domain.Services.Write;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Recommendations.EventHandlers.IntegrationEvents;

public sealed class StoryUniqueReadCountIncreasedIntegrationEventHandler(
    IStorySearchIndexService storyService,
    IStorySearchRepository storySearchRepository,
    IUserPreferencesIndexService userPreferencesIndexService,
    ILogger<StoryUniqueReadCountIncreasedIntegrationEventHandler> logger)
        : IEventHandler<StoryUniqueReadCountIncreasedIntegrationEvent>
{
    private readonly IStorySearchIndexService _storyService = storyService;
    private readonly IStorySearchRepository _storySearchRepository = storySearchRepository;
    private readonly IUserPreferencesIndexService _userPreferencesIndexService = userPreferencesIndexService;

    public async Task Handle(
        StoryUniqueReadCountIncreasedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var story = await _storySearchRepository.GetByIdAsync(integrationEvent.StoryId);

        if (story is null)
        {
            return;
        }

        await _storyService.AddOrUpdateAsync(story with
        {
            UniqueReads = integrationEvent.UniqueReads,
        });

        await _userPreferencesIndexService.AddOrUpdateAsync(integrationEvent.UserAccountId, integrationEvent.StoryId);

        logger.LogInformation("{Handler} handled.", nameof(StoryUniqueReadCountIncreasedIntegrationEventHandler));
    }
}
