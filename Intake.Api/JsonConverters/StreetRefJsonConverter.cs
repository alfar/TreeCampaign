using System.Text.Json;
using System.Text.Json.Serialization;
using Intake.Domain.ExternalReferences;

namespace Intake.Api.JsonConverters;

public class StreetRefJsonConverter : JsonConverter<StreetRef>
{
    public override StreetRef Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return StreetRef.From(value);
    }

    public override void Write(Utf8JsonWriter writer, StreetRef value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
