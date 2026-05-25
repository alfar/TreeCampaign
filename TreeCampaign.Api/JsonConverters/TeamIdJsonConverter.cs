using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Teams.ValueObjects;

public class TeamIdJsonConverter : JsonConverter<TeamId>
{
    public override TeamId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return TeamId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, TeamId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
