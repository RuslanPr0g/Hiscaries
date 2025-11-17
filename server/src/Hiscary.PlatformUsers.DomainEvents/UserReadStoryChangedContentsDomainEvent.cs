using StackNucleus.DDD.Domain.Events;

namespace Hiscary.PlatformUsers.DomainEvents;

public sealed class UserReadStoryChangedContentsDomainEvent(
    Guid UserAccountId,
    Guid StoryId,
    int NumberOfPages) : BaseIntegrationEvent
{
    public Guid StoryId { get; } = StoryId;
    public Guid UserAccountId { get; } = UserAccountId;
    public int NumberOfPages { get; } = NumberOfPages;
}
