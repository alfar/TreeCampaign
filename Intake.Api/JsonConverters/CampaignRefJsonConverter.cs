using System.Text.Json;
using System.Text.Json.Serialization;
using Intake.Domain.ExternalReferences;

namespace Intake.Api.JsonConverters;

public class CampaignRefJsonConverter : JsonConverter<CampaignRef>
{
    public override CampaignRef Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return CampaignRef.From(value);
    }

    public override void Write(Utf8JsonWriter writer, CampaignRef value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
