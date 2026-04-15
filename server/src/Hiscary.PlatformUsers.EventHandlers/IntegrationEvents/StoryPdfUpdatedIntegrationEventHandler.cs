using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class StoryPdfUpdatedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<StoryPdfUpdatedIntegrationEventHandler> logger)
        : IEventHandler<StoryPdfUpdatedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;

    public async Task Handle(
        StoryPdfUpdatedIntegrationEvent integrationEvent,
        IMessageContext context)
    {
        var users = await _repository.GetUsersWithAnnotatedPdfByStoryId(integrationEvent.StoryId);

        var conflictCount = 0;

        foreach (var user in users)
        {
            if (user.MarkAnnotatedPdfConflict(integrationEvent.StoryId))
            {
                conflictCount++;
            }
        }

        if (conflictCount > 0)
        {
            await _repository.SaveChanges();
        }

        logger.LogInformation(
            "{Handler} handled. Marked conflict for {ConflictCount} out of {TotalUserCount} user(s) on StoryId {StoryId}.",
            nameof(StoryPdfUpdatedIntegrationEventHandler),
            conflictCount,
            users.Count,
            integrationEvent.StoryId);
    }

}
