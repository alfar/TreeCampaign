using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Campaigns.ValueObjects;

public class CollectionCampaignIdJsonConverter : JsonConverter<CampaignId>
{
    public override CampaignId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return CampaignId.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        CampaignId value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
