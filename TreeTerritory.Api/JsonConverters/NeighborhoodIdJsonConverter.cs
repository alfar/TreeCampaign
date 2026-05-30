using System.Text.Json;
using System.Text.Json.Serialization;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;

namespace TreeTerritory.Api.JsonConverters;

public class NeighborhoodIdJsonConverter : JsonConverter<NeighborhoodId>
{
    public override NeighborhoodId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return NeighborhoodId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, NeighborhoodId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
