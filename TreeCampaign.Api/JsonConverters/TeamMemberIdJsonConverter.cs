using System.Text.Json;
using System.Text.Json.Serialization;
using TreeCampaign.Domain.TeamMembers.ValueObjects;

public class TeamMemberIdJsonConverter : JsonConverter<TeamMemberId>
{
    public override TeamMemberId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return TeamMemberId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, TeamMemberId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
