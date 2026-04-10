using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.DomainEvents;

public sealed class StoryTotalPagesChangedDomainEvent(Guid StoryId, int NewTotalPages) : BaseDomainEvent
{
    public Guid StoryId { get; } = StoryId;
    public int NewTotalPages { get; } = NewTotalPages;
}
