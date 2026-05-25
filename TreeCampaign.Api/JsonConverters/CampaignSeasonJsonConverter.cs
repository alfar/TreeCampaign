using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Campaigns.ValueObjects;

public class CampaignSeasonJsonConverter : JsonConverter<CampaignSeason>
{
    public override CampaignSeason Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetInt32();
        return CampaignSeason.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        CampaignSeason value,
        JsonSerializerOptions options
    )
    {
        writer.WriteNumberValue(value.Year);
    }
}
