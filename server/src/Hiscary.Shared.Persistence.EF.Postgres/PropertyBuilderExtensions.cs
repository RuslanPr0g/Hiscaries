using Hiscary.Shared.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Hiscary.Shared.Persistence.EF.Postgres;

public static class PropertyBuilderExtensions
{
    private static readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.General);

    public static PropertyBuilder<ImageContainer?> HasImageContainerConversion(
        this PropertyBuilder<ImageContainer?> builder)
    {
        return builder.HasConversion(
            v => JsonSerializer.Serialize(v, _serializerOptions),
            v => DeserializeWithFallback(v));
    }

    private static ImageContainer DeserializeWithFallback(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return ImageContainer.Empty;

        var deserializers = new Func<string, ImageContainer>[]
        {
            DeserializeAsImageContainer,
            DeserializeAsDictionary
        };

        foreach (var deserialize in deserializers)
        {
            try
            {
                var result = deserialize(json);
                if (result is not null && result.Urls.Count > 0) return result;
            }
            catch
            {
                // Ignore and try next
            }
        }

        return ImageContainer.Empty;
    }

    private static ImageContainer DeserializeAsImageContainer(string json) =>
        JsonSerializer.Deserialize<ImageContainer>(json, _serializerOptions) ?? ImageContainer.Empty;

    private static ImageContainer DeserializeAsDictionary(string json)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _serializerOptions);
        return dict is not null ? new ImageContainer(dict) : ImageContainer.Empty;
    }
}