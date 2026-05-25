namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopMarkedUnresolved(StopId Id, ReasonText UnresolvedReason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
