using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.ExternalReferences;

public class StreetSectionRefJsonConverter : JsonConverter<StreetSectionRef>
{
    public override StreetSectionRef Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return StreetSectionRef.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        StreetSectionRef value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
