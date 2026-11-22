using System.Text.Json.Serialization;

namespace TreeTerritory.Domain.StreetSections;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrailerSize
{
    Small = 0,
    Boogie = 1,
    Large = 2,

    /// <summary>
    /// No trailer can enter the street; trees are gathered by a walking team on foot.
    /// Smallest in access terms, but appended last so persisted byte values stay stable.
    /// </summary>
    Runner = 3,
}
