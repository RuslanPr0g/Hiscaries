using Hiscary.Media.DocumentTools;
using Hiscary.Media.IntegrationEvents.Incoming;
using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.FileStorage;
using Hiscary.Shared.Domain.Options;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using Wolverine;

namespace Hiscary.Media.EventHandlers.IntegrationEvents;

public sealed class ConsolidateDocumentRequestedIntegrationEventHandler(
    IEventPublisher publisher,
    IBlobStorageService blobStorageService,
    IDocumentTool documentTool,
    ServiceUrls serviceUrls,
    ILogger<ConsolidateDocumentRequestedIntegrationEventHandler> logger)
    : IEventHandler<ConsolidateDocumentRequestedIntegrationEvent>
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;
    private readonly IDocumentTool _documentTool = documentTool;
    private readonly ServiceUrls _serviceUrls = serviceUrls;
    private readonly ILogger<ConsolidateDocumentRequestedIntegrationEventHandler> _logger = logger;

    public async Task Handle(
        ConsolidateDocumentRequestedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var requesterId = integrationEvent.RequesterId;
        var pdfFileName = integrationEvent.PdfFileName;

        try
        {
            var pdfExists = await _blobStorageService.ExistsAsync("documents", pdfFileName);

            if (!pdfExists)
            {
                _logger.LogWarning("PDF file {PdfFileName} not found for requester {RequesterId}", pdfFileName, requesterId);
                return;
            }

            var (stream, _) = await _blobStorageService.DownloadAsync("documents", pdfFileName);
            using var contentStream = new MemoryStream();
            await stream.CopyToAsync(contentStream);
            var content = contentStream.ToArray();
            var fileSize = content.Length;

            contentStream.Position = 0;
            var documentContent = _documentTool.FileStreamToContent(contentStream, null, null);
            var pageCount = documentContent.Pages.Count;

            var pdfUrl = _serviceUrls.GetDocumentsUrl(pdfFileName);

            await _publisher.Publish(new DocumentConsolidatedIntegrationEvent(requesterId, pdfUrl, fileSize, pageCount));

            _logger.LogInformation("Document consolidated for requester {RequesterId}", requesterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consolidating document for requester {RequesterId}", requesterId);
        }
    }
}
