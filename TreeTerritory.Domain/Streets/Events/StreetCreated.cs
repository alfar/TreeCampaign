using Common.Domain.Abstractions;
using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.Domain.Streets.Events;

public sealed record StreetCreated(StreetId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
