using Common.Domain.Abstractions;
using TreeCampaign.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops.ValueObjects;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopMarkedUnresolved(StopId Id, ReasonText UnresolvedReason, CampaignId CampaignId) : IDomainEvent, ITreeCampaignEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
