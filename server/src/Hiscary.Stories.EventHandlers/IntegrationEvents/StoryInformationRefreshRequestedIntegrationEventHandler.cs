using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.IntegrationEvents.Incoming;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Hiscary.Stories.IntegrationEvents.Outgoing.DTOs;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.ClientModels;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.Stories.EventHandlers.IntegrationEvents;

public sealed class StoryInformationRefreshRequestedIntegrationEventHandler(
    IEventPublisher publisher,
    IStoryReadRepository repository,
    ILogger<StoryInformationRefreshRequestedIntegrationEventHandler> logger)
        : IEventHandler<StoryInformationRefreshRequestedIntegrationEvent>
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IStoryReadRepository _repository = repository;

    public async Task Handle(
        StoryInformationRefreshRequestedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var storyId = integrationEvent.RequesterId;

        var stories = await _repository.GetAllStories(new ClientQueryableModel
        {
            StartIndex = integrationEvent.Index,
            ItemsCount = integrationEvent.ChunkSize,
            SortAsc = false,
            SortProperty = "DatePublished"
        });

        await _publisher.Publish(new StoryInformationRefreshChunkProcessedIntegrationEvent(
            integrationEvent.RequesterId,
            stories.Items.Select(story => new StoryChunk
            {
                Id = story.Id,
                Title = story.Title,
                Description = story.Description,
                Genres = story.GenreNames,
                LibraryId = story.LibraryId,
                PublishedDate = story.DatePublished,
                UniqueReads = story.UniqueReads
            }).ToArray(),
            integrationEvent.Index,
            integrationEvent.ChunkSize));

        logger.LogInformation("{Handler} handled.", nameof(StoryInformationRefreshRequestedIntegrationEventHandler));
    }
}
