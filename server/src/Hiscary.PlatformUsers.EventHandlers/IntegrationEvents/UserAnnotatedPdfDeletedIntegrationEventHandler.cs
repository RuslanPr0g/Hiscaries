using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class UserAnnotatedPdfDeletedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<UserAnnotatedPdfDeletedIntegrationEventHandler> logger)
        : IEventHandler<UserAnnotatedPdfDeletedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;

    public async Task Handle(
        UserAnnotatedPdfDeletedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var user = await _repository.GetByUserAccountId(integrationEvent.UserAccountId);

        if (user is null)
        {
            logger.LogWarning(
                "{Handler}: PlatformUser not found for UserAccountId {UserAccountId}.",
                nameof(UserAnnotatedPdfDeletedIntegrationEventHandler),
                integrationEvent.UserAccountId);
            return;
        }

        var removed = user.RemoveAnnotatedPdf(integrationEvent.StoryId);

        if (removed)
        {
            await _repository.SaveChanges();
        }

        logger.LogInformation("{Handler} handled.", nameof(UserAnnotatedPdfDeletedIntegrationEventHandler));
    }
}
