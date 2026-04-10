using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Incoming;

public sealed class ConsolidateDocumentRequestedIntegrationEvent(
    Guid RequesterId,
    string PdfFileName
) : BaseIntegrationEvent
{
    public Guid RequesterId { get; set; } = RequesterId;
    public string PdfFileName { get; set; } = PdfFileName;
}
