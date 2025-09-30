using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Incoming;

public sealed class StoryInformationRefreshRequestedIntegrationEvent(
    Guid RequesterId,
    long Index,
    long ChunkSize) : BaseIntegrationEvent
{
    public Guid RequesterId { get; } = RequesterId;
    public long Index { get; } = Index;
    public long ChunkSize { get; } = ChunkSize;
}
