using Hiscary.Shared.Domain.ValueObjects;

namespace Hiscary.Shared.Domain.ReadModels;

public sealed class ImageUrlsDto
{
    public string? Small { get; init; }
    public string? Medium { get; init; }
    public string? Large { get; init; }

    public static ImageUrlsDto? FromDomainModel(ImageContainer? images)
    {
        if (images is null)
        {
            return null;
        }

        return new ImageUrlsDto
        {
            Small = images.GetUrl("small"),
            Medium = images.GetUrl("medium"),
            Large = images.GetUrl("large")
        };
    }
}
