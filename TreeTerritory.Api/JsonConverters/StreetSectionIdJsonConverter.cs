using System.Text.Json;
using System.Text.Json.Serialization;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Api.JsonConverters;

public class StreetSectionIdJsonConverter : JsonConverter<StreetSectionId>
{
    public override StreetSectionId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return StreetSectionId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, StreetSectionId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
