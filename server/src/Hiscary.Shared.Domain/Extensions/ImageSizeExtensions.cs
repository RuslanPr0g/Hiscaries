using StackNucleus.DDD.Domain.Images;

namespace Hiscary.Shared.Domain.Extensions;

public static class ImageSizeExtensions
{
    public static ImageSize[] AllImageSizes()
    {
        return [ImageSize.Small, ImageSize.Medium, ImageSize.Large];
    }
}
