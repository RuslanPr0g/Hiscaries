using Hiscary.Media.DocumentTools;
using Hiscary.Media.IntegrationEvents.Incoming;
using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.FileStorage;
using Hiscary.Shared.Domain.Options;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using System.Net.Mime;
using Wolverine;

namespace Hiscary.Media.EventHandlers.IntegrationEvents;

public sealed class GenerateAndUploadDocumentRequestedIntegrationEventHandler(
    IEventPublisher publisher,
    IBlobStorageService blobStorageService,
    IPdfGenerator pdfGenerator,
    ServiceUrls serviceUrls,
    ILogger<GenerateAndUploadDocumentRequestedIntegrationEventHandler> logger)
    : IEventHandler<GenerateAndUploadDocumentRequestedIntegrationEvent>
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;
    private readonly IPdfGenerator _pdfGenerator = pdfGenerator;
    private readonly ServiceUrls _serviceUrls = serviceUrls;
    private readonly ILogger<GenerateAndUploadDocumentRequestedIntegrationEventHandler> _logger = logger;

    public async Task Handle(
        GenerateAndUploadDocumentRequestedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var requesterId = integrationEvent.RequesterId;
        var htmlPages = integrationEvent.HtmlPages.ToList();

        try
        {
            var pdfBytes = await _pdfGenerator.GeneratePdfFromHtmlAsync(htmlPages);

            var pdfFileName = $"{requesterId}.pdf";
            await _blobStorageService.UploadAsync(
                "documents",
                pdfFileName,
                pdfBytes,
                MediaTypeNames.Application.Pdf);

            var pdfUrl = _serviceUrls.GetDocumentsUrl(pdfFileName);

            await _publisher.Publish(new DocumentGeneratedAndUploadedIntegrationEvent(requesterId, pdfUrl, pdfBytes.Length, htmlPages.Count));

            _logger.LogInformation("Document generated and uploaded for requester {RequesterId}", requesterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating and uploading document for requester {RequesterId}", requesterId);
        }
    }
}
