namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopReopened(StopId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
