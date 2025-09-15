using StackNucleus.DDD.Domain.Events;
using StackNucleus.DDD.Domain.Images;

namespace Hiscary.Media.IntegrationEvents.Incoming;

public sealed class ImageUploadRequestedIntegrationEvent(
    byte[] Content,
    Guid RequesterId,
    string Type,
    ImageSize[] Sizes
    ) : BaseIntegrationEvent
{
    public byte[] Content { get; set; } = Content;
    public Guid RequesterId { get; set; } = RequesterId;
    public string Type { get; set; } = Type;
    public ImageSize[] Sizes { get; set; } = Sizes is null || Sizes.Length == 0 ? [ImageSize.Large] : Sizes;
}
