using Common.Domain.Abstractions;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Domain.StreetSections.Events;

public sealed record StreetSectionCreated(StreetSectionId Id, NeighborhoodId NeighborhoodId, StreetId StreetId, HouseNumber? StartNumber, HouseNumber? EndNumber, int SortOrder, Direction Direction) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
