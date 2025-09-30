using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryUniqueReadCountIncreasedIntegrationEvent(
    Guid StoryId,
    Guid UserAccountId,
    long UniqueReads) : BaseIntegrationEvent
{
    public Guid StoryId { get; } = StoryId;
    public Guid UserAccountId { get; } = UserAccountId;
    public long UniqueReads { get; } = UniqueReads;
}
