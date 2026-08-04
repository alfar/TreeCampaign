using System.Text.Json.Serialization;

namespace TreeTerritory.Domain.StreetSections;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrailerSize
{
    Small,
    Large,
    Boogie,
}
