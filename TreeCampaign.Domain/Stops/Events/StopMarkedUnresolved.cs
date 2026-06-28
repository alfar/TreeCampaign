using Common.Domain.Abstractions;
using TreeCampaign.Domain.Stops.ValueObjects;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopMarkedUnresolved(StopId Id, ReasonText UnresolvedReason, Guid CampaignId) : IDomainEvent, ICampaignScoped
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
