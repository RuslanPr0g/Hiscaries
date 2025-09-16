using StackNucleus.DDD.Domain.Events;
using StackNucleus.DDD.Domain.Images;

namespace Hiscary.Media.IntegrationEvents.Outgoing;

public sealed class ImageUploadedIntegrationEvent(
    Guid RequesterId,
    ImageUrlToSize[] ImageUrls
    ) : BaseIntegrationEvent
{
    public Guid RequesterId { get; set; } = RequesterId;
    public ImageUrlToSize[] ImageUrls { get; set; } = ImageUrls;
}
