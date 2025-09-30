using Hiscary.Stories.IntegrationEvents.Outgoing.DTOs;
using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryInformationRefreshChunkProcessedIntegrationEvent(
    Guid RequesterId,
    StoryChunk[] Stories,
    long Index,
    long ChunkSize) : BaseIntegrationEvent
{
    public Guid RequesterId { get; } = RequesterId;
    public StoryChunk[] Stories { get; } = Stories;
    public long Index { get; } = Index;
    public long ChunkSize { get; } = ChunkSize;
}
