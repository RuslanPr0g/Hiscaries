using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryPdfUpdatedIntegrationEvent(
    Guid StoryId,
    string NewPdfUrl) : BaseIntegrationEvent
{
    public Guid StoryId { get; } = StoryId;
    public string NewPdfUrl { get; } = NewPdfUrl;
}
