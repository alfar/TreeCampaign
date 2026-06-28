using Common.Domain.Abstractions;
using TreeCampaign.Domain.Stops.ValueObjects;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopCreated(StopId Id, Address Address, TreeCount Amount, Guid CampaignId) : IDomainEvent, ICampaignScoped
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
