using System.Text.Json;
using System.Text.Json.Serialization;
using Access.Domain.ScoutGroups.ValueObjects;

namespace Access.Api.JsonConverters;

public class ScoutGroupIdJsonConverter : JsonConverter<ScoutGroupId>
{
    public override ScoutGroupId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return ScoutGroupId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, ScoutGroupId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
