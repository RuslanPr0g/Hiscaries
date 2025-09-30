using Hiscary.Stories.IntegrationEvents.Outgoing.DTOs;
using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryInformationRefreshChunkProcessedIntegrationEvent(
    Guid RequesterId,
    StoryChunk[] Stories,
    int Index,
    int ChunkSize) : BaseIntegrationEvent
{
    public Guid RequesterId { get; } = RequesterId;
    public StoryChunk[] Stories { get; } = Stories;
    public int Index { get; } = Index;
    public int ChunkSize { get; } = ChunkSize;
}
