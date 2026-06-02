using Common.Domain.Abstractions;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopCreated(StopId Id, Address Address, TreeCount Amount) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
