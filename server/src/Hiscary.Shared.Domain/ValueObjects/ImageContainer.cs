using StackNucleus.DDD.Domain.Images.Uploaders;

namespace Hiscary.Shared.Domain.ValueObjects;

public sealed class ImageContainer : IEquatable<ImageContainer>
{
    public static ImageContainer Empty => new ImageContainer();

    private ImageContainer()
    {
        Urls = [];
    }

    public ImageContainer(Dictionary<string, string>? urls = null)
    {
        Urls = urls is not null
            ? urls
            : [];
    }

    public static ImageContainer FromImageUrlToSize(params ImageUrlToSize[] imageUrlToSizes)
    {
        if (imageUrlToSizes is null || imageUrlToSizes.Length == 0)
            return Empty;

        var dict = imageUrlToSizes
            .Where(i => !string.IsNullOrWhiteSpace(i.Url))
            .ToDictionary(
                i => i.Size.ToString().ToLowerInvariant(),
                i => i.Url);

        return new ImageContainer(dict);
    }

    public Dictionary<string, string> Urls { get; private set; } = new();

    public string? GetUrl(string size)
    {
        return Urls.TryGetValue(size, out var url) ? url : null;
    }

    public ImageContainer WithUrl(string size, string url)
    {
        var dict = new Dictionary<string, string>(Urls) { [size] = url };
        return new ImageContainer(dict);
    }

    public bool Equals(ImageContainer? other)
    {
        if (other is null) return false;
        return Urls.OrderBy(kv => kv.Key).SequenceEqual(other.Urls.OrderBy(kv => kv.Key));
    }

    public override bool Equals(object? obj) => Equals(obj as ImageContainer);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var kv in Urls.OrderBy(kv => kv.Key))
            {
                hash = hash * 23 + kv.Key.GetHashCode();
                hash = hash * 23 + (kv.Value?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
