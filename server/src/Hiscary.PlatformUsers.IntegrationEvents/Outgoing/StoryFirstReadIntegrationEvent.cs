using StackNucleus.DDD.Domain.Events;

namespace Hiscary.PlatformUsers.IntegrationEvents.Outgoing;

public sealed class StoryFirstReadIntegrationEvent(
    Guid UserAccountId,
    Guid StoryId) : BaseIntegrationEvent
{
    public Guid UserAccountId { get; } = UserAccountId;
    public Guid StoryId { get; } = StoryId;
}
