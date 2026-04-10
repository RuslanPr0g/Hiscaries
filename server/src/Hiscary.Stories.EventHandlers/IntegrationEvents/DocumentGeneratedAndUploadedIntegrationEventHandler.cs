using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.ValueObjects;
using Hiscary.Stories.Domain.DataAccess;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Stories.EventHandlers.IntegrationEvents;

public sealed class DocumentGeneratedAndUploadedIntegrationEventHandler(
    IStoryWriteRepository repository,
    ILogger<DocumentGeneratedAndUploadedIntegrationEventHandler> logger)
    : IEventHandler<DocumentGeneratedAndUploadedIntegrationEvent>
{
    private readonly IStoryWriteRepository _repository = repository;
    private readonly ILogger<DocumentGeneratedAndUploadedIntegrationEventHandler> _logger = logger;

    public async Task Handle(
        DocumentGeneratedAndUploadedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var storyId = integrationEvent.RequesterId;

        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found for document generation", storyId);
            return;
        }

        var externalPdf = ExternalPdf.FromUrlAndSize(integrationEvent.PdfUrl, integrationEvent.PdfSize);
        story.SetExternalPdf(externalPdf, integrationEvent.PageCount);

        await _repository.SaveChanges();

        _logger.LogInformation("Document generated and external PDF cleared for story {StoryId}", storyId);
    }
}
