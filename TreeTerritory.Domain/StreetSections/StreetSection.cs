using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Domain.StreetSections;

public class StreetSection
{
    public required StreetSectionId Id { get; init; }
    public required NeighborhoodId NeighborhoodId { get; init; }
    public required StreetId StreetId { get; init; }

    public required HouseNumber StartHouseNumber { get; init; }
    public required HouseNumber EndHouseNumber { get; init; }

    public int SortOrder { get; private set; } = 0;
    public Direction Direction { get; private set; } = Direction.Ascending;

    internal static StreetSection Create(
        NeighborhoodId neighborhoodId,
        StreetId streetId,
        HouseNumber startHouseNumber,
        HouseNumber endHouseNumber,
        int sortOrder,
        Direction direction)
    {
        var comparison = startHouseNumber.CompareTo(endHouseNumber);

        return new StreetSection
        {
            Id = StreetSectionId.From(Guid.NewGuid()),
            NeighborhoodId = neighborhoodId,
            StreetId = streetId,
            StartHouseNumber = comparison <= 0 ? startHouseNumber : endHouseNumber,
            EndHouseNumber = comparison <= 0 ? endHouseNumber : startHouseNumber,
            SortOrder = sortOrder,
            Direction = direction
        };
    }

    private StreetSection() { }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void UpdateDirection(Direction direction)
    {
        Direction = direction;
    }
}