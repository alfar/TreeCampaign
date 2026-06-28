using Common.Domain.Abstractions;
using TreeCampaign.Domain.Stops.ValueObjects;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopUnassigned(StopId Id, Guid CampaignId) : IDomainEvent, ICampaignScoped
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
