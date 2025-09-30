using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Services.Write;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Recommendations.EventHandlers.IntegrationEvents;

public sealed class StoryUpdatedIntegrationEventHandler(
    IStorySearchIndexService service,
    ILogger<StoryUpdatedIntegrationEventHandler> logger)
        : IEventHandler<StoryUpdatedIntegrationEvent>
{
    private readonly IStorySearchIndexService _service = service;

    public async Task Handle(
        StoryUpdatedIntegrationEvent integrationEvent, IMessageContext context)
    {
        await _service.AddOrUpdateAsync(new Story
        {
            Id = integrationEvent.StoryId,
            Title = integrationEvent.Title,
            Description = integrationEvent.Description,
            Genres = integrationEvent.Genres.ToHashSet(),
            LibraryId = integrationEvent.LibraryId,
            PublishedDate = integrationEvent.PublishedDate,
            UniqueReads = integrationEvent.UniqueReads,
        });

        logger.LogInformation("{Handler} handled.", nameof(StoryUpdatedIntegrationEventHandler));
    }
}
