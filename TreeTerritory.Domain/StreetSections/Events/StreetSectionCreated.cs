using Common.Domain.Abstractions;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Domain.StreetSections.Events;

public sealed record StreetSectionCreated(
    StreetSectionId Id,
    NeighborhoodId NeighborhoodId,
    StreetId StreetId,
    HouseNumber? EvenStartNumber,
    HouseNumber? EvenEndNumber,
    HouseNumber? OddStartNumber,
    HouseNumber? OddEndNumber,
    int SortOrder,
    Direction Direction,
    TrailerSize MaxTrailerSize) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
