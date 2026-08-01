using Common.Domain.Abstractions;
using Intake.Domain.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Events;

public sealed record OrderSettled(OrderId Id, CampaignRef CampaignId, TerritoryRef TerritoryId) : IDomainEvent, IIntakeEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
