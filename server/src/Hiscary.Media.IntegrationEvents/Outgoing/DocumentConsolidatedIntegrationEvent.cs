using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Outgoing;

public sealed class DocumentConsolidatedIntegrationEvent(
    Guid RequesterId,
    string PdfUrl,
    long FileSize,
    int PageCount
) : BaseIntegrationEvent
{
    public Guid RequesterId { get; set; } = RequesterId;
    public string PdfUrl { get; set; } = PdfUrl;
    public long FileSize { get; set; } = FileSize;
    public int PageCount { get; set; } = PageCount;
}
