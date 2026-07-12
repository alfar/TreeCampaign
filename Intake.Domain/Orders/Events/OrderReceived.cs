using Common.Domain.Abstractions;
using Intake.Domain.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Events;

public sealed record OrderReceived(OrderId Id, CampaignRef CampaignId, Sender Sender, MoneyAmount Amount, DateTimeOffset OrderDate, string Message) : IDomainEvent, IIntakeEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
