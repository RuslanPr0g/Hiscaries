using StackNucleus.DDD.Domain.Events;
using StackNucleus.DDD.Domain.Images;

namespace Hiscary.Notifications.IntegrationEvents.Incoming;

public sealed class NotificationReferenceObjectIdPreviewChangedIntegrationEvent(
    Guid ObjectReferenceId,
    ImageUrlToSize[] ImageUrls) : BaseIntegrationEvent
{
    public Guid ObjectReferenceId { get; set; } = ObjectReferenceId;
    public ImageUrlToSize[] ImageUrls { get; set; } = ImageUrls;
}
