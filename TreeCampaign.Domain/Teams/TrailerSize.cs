using System.Text.Json.Serialization;

namespace TreeCampaign.Domain.Teams;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrailerSize
{
    Small,
    Large,
    Boogie,
}
