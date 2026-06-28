using System.Text.Json.Serialization;

namespace TreeCampaign.Domain.Teams;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TeamStatus : byte
{
    Active = 0,
    OnBreak = 1,
    TrailerFull = 2,
}
