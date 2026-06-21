using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Stops.ValueObjects;

public class StopIdJsonConverter : JsonConverter<StopId>
{
    public override StopId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return StopId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, StopId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
