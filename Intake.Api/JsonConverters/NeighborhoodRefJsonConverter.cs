using System.Text.Json;
using System.Text.Json.Serialization;
using Intake.Domain.ExternalReferences;

namespace Intake.Api.JsonConverters;

public class NeighborhoodRefJsonConverter : JsonConverter<NeighborhoodRef>
{
    public override NeighborhoodRef Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return NeighborhoodRef.From(value);
    }

    public override void Write(Utf8JsonWriter writer, NeighborhoodRef value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
