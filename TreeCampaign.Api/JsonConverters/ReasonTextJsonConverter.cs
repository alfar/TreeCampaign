using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Stops;

public class ReasonTextJsonConverter : JsonConverter<ReasonText>
{
    public override ReasonText Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString();
        return value is null ? ReasonText.Empty : ReasonText.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        ReasonText value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Text.ToString());
    }
}
