using System.Text.Json;
using System.Text.Json.Serialization;
using TreeTerritory.Domain.ExternalReferences;

public class ScoutGroupRefJsonConverter : JsonConverter<ScoutGroupRef>
{
    public override ScoutGroupRef Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return ScoutGroupRef.From(value);
    }

    public override void Write(Utf8JsonWriter writer, ScoutGroupRef value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
