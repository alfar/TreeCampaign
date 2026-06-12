using Common.Domain.Abstractions;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.Events;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.Events;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Domain.StreetSections;

public class StreetSection : IHasDomainEvents
{
    private readonly List<IDomainEvent> _newEvents = new();
    public IReadOnlyCollection<IDomainEvent> NewEvents => _newEvents.AsReadOnly();

    protected void Raise(IDomainEvent @event)
    {
        _newEvents.Add(@event);
    }

    public void ClearEvents()
    {
        _newEvents.Clear();
    }

    public required StreetSectionId Id { get; init; }
    public required NeighborhoodId NeighborhoodId { get; init; }
    public required StreetId StreetId { get; init; }

    public HouseNumber? StartHouseNumber { get; init; }
    public HouseNumber? EndHouseNumber { get; init; }

    public int SortOrder { get; private set; } = 0;
    public Direction Direction { get; private set; } = Direction.Ascending;

    internal static StreetSection Create(
        NeighborhoodId neighborhoodId,
        StreetId streetId,
        HouseNumber? startHouseNumber,
        HouseNumber? endHouseNumber,
        int sortOrder,
        Direction direction)
    {
        var comparison = (startHouseNumber is null || endHouseNumber is null) ? -1 : startHouseNumber.CompareTo(endHouseNumber);

        var section = new StreetSection
        {
            Id = StreetSectionId.From(Guid.NewGuid()),
            NeighborhoodId = neighborhoodId,
            StreetId = streetId,
            StartHouseNumber = comparison <= 0 ? startHouseNumber : endHouseNumber,
            EndHouseNumber = comparison <= 0 ? endHouseNumber : startHouseNumber,
            SortOrder = sortOrder,
            Direction = direction
        };

        section.Raise(new StreetSectionCreated(section.Id, neighborhoodId, streetId, startHouseNumber, endHouseNumber, sortOrder, direction));

        return section;
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

    public bool ContainsHouseNumber(HouseNumber houseNumber)
    {        
        return (StartHouseNumber?.CompareTo(houseNumber) ?? -1) <= 0 && (EndHouseNumber?.CompareTo(houseNumber) ?? 1) >= 0;
    }
}