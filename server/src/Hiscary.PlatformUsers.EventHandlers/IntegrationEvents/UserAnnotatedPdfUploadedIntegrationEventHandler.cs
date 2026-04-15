using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class UserAnnotatedPdfUploadedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<UserAnnotatedPdfUploadedIntegrationEventHandler> logger)
        : IEventHandler<UserAnnotatedPdfUploadedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;

    public async Task Handle(
        UserAnnotatedPdfUploadedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var user = await _repository.GetByUserAccountId(integrationEvent.UserAccountId);

        if (user is null)
        {
            logger.LogWarning(
                "{Handler}: PlatformUser not found for UserAccountId {UserAccountId}.",
                nameof(UserAnnotatedPdfUploadedIntegrationEventHandler),
                integrationEvent.UserAccountId);
            return;
        }

        user.SetAnnotatedPdf(integrationEvent.StoryId, integrationEvent.PdfUrl);

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(UserAnnotatedPdfUploadedIntegrationEventHandler));
    }
}
