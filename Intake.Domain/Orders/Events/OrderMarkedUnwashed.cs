using Common.Domain.Abstractions;
using Intake.Domain.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Events;

public sealed record OrderMarkedUnwashed(OrderId Id, CampaignRef CampaignId) : IDomainEvent, IIntakeEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
