using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Outgoing;

public sealed class UserAnnotatedPdfDeletedIntegrationEvent(
    Guid UserAccountId,
    Guid StoryId
) : BaseIntegrationEvent
{
    public Guid UserAccountId { get; set; } = UserAccountId;
    public Guid StoryId { get; set; } = StoryId;
}
