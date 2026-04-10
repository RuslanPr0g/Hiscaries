using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.ValueObjects;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Stories;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.Stories.EventHandlers.IntegrationEvents;

public sealed class DocumentConsolidatedIntegrationEventHandler(
    IStoryWriteRepository repository,
    ILogger<DocumentConsolidatedIntegrationEventHandler> logger)
    : IEventHandler<DocumentConsolidatedIntegrationEvent>
{
    private readonly IStoryWriteRepository _repository = repository;
    private readonly ILogger<DocumentConsolidatedIntegrationEventHandler> _logger = logger;

    public async Task Handle(
        DocumentConsolidatedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var storyId = integrationEvent.RequesterId;
        var pdfUrl = integrationEvent.PdfUrl;
        var fileSize = integrationEvent.FileSize;
        var pageCount = integrationEvent.PageCount;

        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found for document consolidation", storyId);
            return;
        }

        var externalPdf = ExternalPdf.FromUrlAndSize(pdfUrl, fileSize);
        story.SetExternalPdf(externalPdf, pageCount);

        await _repository.SaveChanges();

        _logger.LogInformation("External PDF consolidated for story {StoryId}", storyId);
    }
}
