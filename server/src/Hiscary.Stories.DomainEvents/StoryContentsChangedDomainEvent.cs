using StackNucleus.DDD.Domain.Events;

namespace Hiscary.PlatformUsers.DomainEvents;

public sealed class StoryContentsChangedDomainEvent(Guid StoryId, int NumberOfPages) : BaseDomainEvent
{
    public Guid StoryId { get; } = StoryId;
    public int NumberOfPages { get; } = NumberOfPages;
}
