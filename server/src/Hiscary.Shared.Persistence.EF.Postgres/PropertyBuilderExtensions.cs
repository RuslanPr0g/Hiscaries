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
            v => new ImageContainer(JsonSerializer.Deserialize<Dictionary<string, string>>(v, _serializerOptions)) ?? ImageContainer.Empty
        );
    }
}