using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Stops.ValueObjects;

public class TreeCountDeltaJsonConverter : JsonConverter<TreeCountDelta>
{
    public override TreeCountDelta Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetInt32();
        return TreeCountDelta.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        TreeCountDelta value,
        JsonSerializerOptions options
    )
    {
        writer.WriteNumberValue(value.Value);
    }
}
