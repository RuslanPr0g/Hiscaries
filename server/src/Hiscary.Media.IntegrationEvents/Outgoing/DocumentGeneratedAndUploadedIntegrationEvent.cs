using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Outgoing;

public sealed class DocumentGeneratedAndUploadedIntegrationEvent(
    Guid RequesterId,
    string PdfUrl,
    int PdfSize,
    int PageCount
) : BaseIntegrationEvent
{
    public Guid RequesterId { get; set; } = RequesterId;
    public string PdfUrl { get; set; } = PdfUrl;
    public int PdfSize { get; set; } = PdfSize;
    public int PageCount { get; set; } = PageCount;
}
