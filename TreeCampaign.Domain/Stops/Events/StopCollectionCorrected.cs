using Common.Domain.Abstractions;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopCollectionCorrected(StopId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
