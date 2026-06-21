using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Stops.ValueObjects;

public class TreeCountJsonConverter : JsonConverter<TreeCount>
{
    public override TreeCount Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetInt32();
        return TreeCount.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        TreeCount value,
        JsonSerializerOptions options
    )
    {
        writer.WriteNumberValue(value.Value);
    }
}
