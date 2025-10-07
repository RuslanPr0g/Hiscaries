using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Services.Write;
using Hiscary.Stories.IntegrationEvents.Incoming;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.Recommendations.EventHandlers.IntegrationEvents;

public sealed class StoryInformationRefreshChunkProcessedIntegrationEventHandler(
    IEventPublisher publisher,
    IStorySearchIndexService service,
    ILogger<StoryInformationRefreshChunkProcessedIntegrationEventHandler> logger)
        : IEventHandler<StoryInformationRefreshChunkProcessedIntegrationEvent>
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IStorySearchIndexService _service = service;

    public async Task Handle(
        StoryInformationRefreshChunkProcessedIntegrationEvent integrationEvent, IMessageContext context)
    {
        if (integrationEvent.Stories.Length <= 0)
        {
            return;
        }

        // TODO: check the requester id to be sure that our service was the one calling this process
        var response = await _service.AddOrUpdateAsync(integrationEvent.Stories.Select(story =>
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

        await _publisher.Publish(new StoryInformationRefreshRequestedIntegrationEvent(
            integrationEvent.RequesterId,
            integrationEvent.Index + 1,
            integrationEvent.ChunkSize));

        logger.LogInformation("{Handler} handled.", nameof(StoryInformationRefreshChunkProcessedIntegrationEventHandler));
    }
}
