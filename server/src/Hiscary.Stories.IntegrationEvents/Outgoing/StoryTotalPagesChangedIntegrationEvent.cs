using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryTotalPagesChangedIntegrationEvent(
    Guid StoryId,
    int NewTotalPages) : BaseIntegrationEvent
{
    public Guid StoryId { get; } = StoryId;
    public int NewTotalPages { get; } = NewTotalPages;
}
