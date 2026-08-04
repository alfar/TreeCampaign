using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Domain.Neighborhoods;

public class Neighborhood
{
    public required NeighborhoodId Id { get; init; }
    public required TerritoryId TerritoryId { get; init; }
    public required string Name { get; init; }

    private readonly List<StreetSection> _streetSections = [];
    public IReadOnlyList<StreetSection> StreetSections => _streetSections.AsReadOnly();

    public static Neighborhood Create(TerritoryId territoryId, string name)
    {
        return new Neighborhood
        {
            Id = NeighborhoodId.From(Guid.NewGuid()),
            TerritoryId = territoryId,
            Name = name
        };
    }

    public void AddStreetSection(
        StreetId streetId,
        HouseNumber? evenStartHouseNumber,
        HouseNumber? evenEndHouseNumber,
        HouseNumber? oddStartHouseNumber,
        HouseNumber? oddEndHouseNumber,
        int sortOrder,
        Direction direction,
        TrailerSize maxTrailerSize = TrailerSize.Boogie)
    {
        var section = StreetSection.Create(this.Id, streetId, evenStartHouseNumber, evenEndHouseNumber, oddStartHouseNumber, oddEndHouseNumber, sortOrder, direction, maxTrailerSize);

        // Add invariant checks here (no overlaps, etc.)
        _streetSections.Add(section);
    }

    public void UpdateStreetSection(
        StreetSectionId streetSectionId,
        HouseNumber? evenStartHouseNumber,
        HouseNumber? evenEndHouseNumber,
        HouseNumber? oddStartHouseNumber,
        HouseNumber? oddEndHouseNumber,
        int sortOrder,
        Direction direction,
        TrailerSize maxTrailerSize)
    {
        var section = _streetSections.SingleOrDefault(s => s.Id == streetSectionId)
            ?? throw new InvalidOperationException($"Street section '{streetSectionId}' not found in neighborhood '{Id}'.");

        section.UpdateHouseNumberRange(evenStartHouseNumber, evenEndHouseNumber, oddStartHouseNumber, oddEndHouseNumber);
        section.UpdateSortOrder(sortOrder);
        section.UpdateDirection(direction);
        section.UpdateMaxTrailerSize(maxTrailerSize);
    }

    public void RemoveStreetSection(StreetSectionId streetSectionId)
    {
        var section = _streetSections.SingleOrDefault(s => s.Id == streetSectionId)
            ?? throw new InvalidOperationException($"Street section '{streetSectionId}' not found in neighborhood '{Id}'.");

        _streetSections.Remove(section);
    }

    private Neighborhood() { }
}
