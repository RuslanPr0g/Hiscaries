using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Incoming;

public sealed class StoryInformationRefreshRequestedIntegrationEvent(
    Guid RequesterId,
    int Index,
    int ChunkSize) : BaseIntegrationEvent
{
    public Guid RequesterId { get; } = RequesterId;
    public int Index { get; } = Index;
    public int ChunkSize { get; } = ChunkSize;
}
