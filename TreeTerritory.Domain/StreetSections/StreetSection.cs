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

    public HouseNumber? EvenStartHouseNumber { get; private set; }
    public HouseNumber? EvenEndHouseNumber { get; private set; }
    public HouseNumber? OddStartHouseNumber { get; private set; }
    public HouseNumber? OddEndHouseNumber { get; private set; }

    public int SortOrder { get; private set; } = 0;
    public Direction Direction { get; private set; } = Direction.Ascending;
    public TrailerSize MaxTrailerSize { get; private set; } = TrailerSize.Boogie;

    internal static StreetSection Create(
        NeighborhoodId neighborhoodId,
        StreetId streetId,
        HouseNumber? evenStartHouseNumber,
        HouseNumber? evenEndHouseNumber,
        HouseNumber? oddStartHouseNumber,
        HouseNumber? oddEndHouseNumber,
        int sortOrder,
        Direction direction,
        TrailerSize maxTrailerSize = TrailerSize.Boogie)
    {
        var (evenStart, evenEnd) = OrderPair(evenStartHouseNumber, evenEndHouseNumber);
        var (oddStart, oddEnd) = OrderPair(oddStartHouseNumber, oddEndHouseNumber);

        var section = new StreetSection
        {
            Id = StreetSectionId.From(Guid.NewGuid()),
            NeighborhoodId = neighborhoodId,
            StreetId = streetId,
            EvenStartHouseNumber = evenStart,
            EvenEndHouseNumber = evenEnd,
            OddStartHouseNumber = oddStart,
            OddEndHouseNumber = oddEnd,
            SortOrder = sortOrder,
            Direction = direction,
            MaxTrailerSize = maxTrailerSize
        };

        section.Raise(new StreetSectionCreated(section.Id, neighborhoodId, streetId, evenStart, evenEnd, oddStart, oddEnd, sortOrder, direction, maxTrailerSize));

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

    public void UpdateMaxTrailerSize(TrailerSize maxTrailerSize)
    {
        MaxTrailerSize = maxTrailerSize;
    }

    public void UpdateHouseNumberRange(
        HouseNumber? evenStartHouseNumber,
        HouseNumber? evenEndHouseNumber,
        HouseNumber? oddStartHouseNumber,
        HouseNumber? oddEndHouseNumber)
    {
        (EvenStartHouseNumber, EvenEndHouseNumber) = OrderPair(evenStartHouseNumber, evenEndHouseNumber);
        (OddStartHouseNumber, OddEndHouseNumber) = OrderPair(oddStartHouseNumber, oddEndHouseNumber);
    }

    public bool ContainsHouseNumber(HouseNumber houseNumber)
    {
        var (start, end) = houseNumber.Number % 2 == 0
            ? (EvenStartHouseNumber, EvenEndHouseNumber)
            : (OddStartHouseNumber, OddEndHouseNumber);

        return (start?.CompareTo(houseNumber) ?? -1) <= 0 && (end?.CompareTo(houseNumber) ?? 1) >= 0;
    }

    private static (HouseNumber? Start, HouseNumber? End) OrderPair(HouseNumber? start, HouseNumber? end)
    {
        if (start is null || end is null) return (start, end);
        return start.CompareTo(end) <= 0 ? (start, end) : (end, start);
    }
}