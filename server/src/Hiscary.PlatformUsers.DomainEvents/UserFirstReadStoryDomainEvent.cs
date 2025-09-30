using StackNucleus.DDD.Domain.Events;

namespace Hiscary.PlatformUsers.DomainEvents;

public sealed class UserFirstReadStoryDomainEvent(
    Guid UserAccountId,
    Guid StoryId) : BaseDomainEvent
{
    public Guid UserAccountId { get; } = UserAccountId;
    public Guid StoryId { get; } = StoryId;
}
