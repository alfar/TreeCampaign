using System.Text.Json.Serialization;

namespace TreeCampaign.Domain.Teams;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TeamKind
{
    Walking,
    Trailer,
}
