using System.Text.Json;
using System.Text.Json.Serialization;
using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.Api.JsonConverters;

public class StreetIdJsonConverter : JsonConverter<StreetId>
{
    public override StreetId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return StreetId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, StreetId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
