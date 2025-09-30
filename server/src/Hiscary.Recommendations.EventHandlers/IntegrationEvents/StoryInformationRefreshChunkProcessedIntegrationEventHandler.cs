using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Services.Write;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Recommendations.EventHandlers.IntegrationEvents;

public sealed class StoryInformationRefreshChunkProcessedIntegrationEventHandler(
    IStorySearchIndexService service,
    ILogger<StoryInformationRefreshChunkProcessedIntegrationEventHandler> logger)
        : IEventHandler<StoryInformationRefreshChunkProcessedIntegrationEvent>
{
    private readonly IStorySearchIndexService _service = service;

    public async Task Handle(
        StoryInformationRefreshChunkProcessedIntegrationEvent integrationEvent, IMessageContext context)
    {
        await _service.AddOrUpdateAsync(integrationEvent.Stories.Select(story =>
            new Story
            {
                Id = story.Id,
                Title = story.Title,
                Description = story.Description,
                Genres = story.Genres.ToHashSet(),
                LibraryId = story.LibraryId,
                PublishedDate = story.PublishedDate,
                UniqueReads = story.UniqueReads,
            }).ToArray());

        logger.LogInformation("{Handler} handled.", nameof(StoryInformationRefreshChunkProcessedIntegrationEventHandler));
    }
}
