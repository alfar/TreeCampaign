using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Domain.Neighborhoods.Services;

public abstract record StreetSectionParsingResult;

public sealed record ParsedStreetSection(
    int LineNumber,
    string StreetName,
    HouseNumber? EvenStartHouseNumber,
    HouseNumber? EvenEndHouseNumber,
    HouseNumber? OddStartHouseNumber,
    HouseNumber? OddEndHouseNumber,
    TrailerSize MaxTrailerSize,
    string NeighborhoodName,
    int SortOrder,
    Direction Direction) : StreetSectionParsingResult;

public sealed record StreetSectionParsingFailed(int LineNumber, string Reason) : StreetSectionParsingResult;

public interface ICsvStreetSectionParser
{
    IReadOnlyList<StreetSectionParsingResult> Parse(string csvContent);
}
