using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.Teams.ValueObjects;

public class TeamNameJsonConverter : JsonConverter<TeamName>
{
    public override TeamName Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString();
        return value is null ? TeamName.Empty : TeamName.From(value);
    }

    public override void Write(Utf8JsonWriter writer, TeamName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
