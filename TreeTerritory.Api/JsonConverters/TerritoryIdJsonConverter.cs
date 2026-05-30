using System.Text.Json;
using System.Text.Json.Serialization;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Api.JsonConverters;
public class TerritoryIdJsonConverter : JsonConverter<TerritoryId>
{
    public override TerritoryId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return TerritoryId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, TerritoryId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
