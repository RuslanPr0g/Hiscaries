using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Incoming;

public sealed class GenerateAndUploadDocumentRequestedIntegrationEvent(
    Guid RequesterId,
    IEnumerable<string> HtmlPages
) : BaseIntegrationEvent
{
    public Guid RequesterId { get; set; } = RequesterId;
    public IEnumerable<string> HtmlPages { get; set; } = HtmlPages;
}
