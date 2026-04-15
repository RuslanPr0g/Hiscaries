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
        StoryPdfUpdatedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var users = await _repository.GetUsersWithAnnotatedPdfByStoryId(integrationEvent.StoryId);

        foreach (var user in users)
        {
            user.MarkAnnotatedPdfConflict(integrationEvent.StoryId);
        }

        if (users.Count > 0)
        {
            await _repository.SaveChanges();
        }

        logger.LogInformation(
            "{Handler} handled. Marked conflict for {UserCount} user(s) on StoryId {StoryId}.",
            nameof(StoryPdfUpdatedIntegrationEventHandler),
            users.Count,
            integrationEvent.StoryId);
    }
}
