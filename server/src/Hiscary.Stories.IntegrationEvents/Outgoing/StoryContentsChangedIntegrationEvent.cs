using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryContentsChangedIntegrationEvent(
    Guid StoryId,
    int NumberOfPages) : BaseIntegrationEvent
{
    public Guid StoryId { get; } = StoryId;
    public int NumberOfPages { get; } = NumberOfPages;
}
