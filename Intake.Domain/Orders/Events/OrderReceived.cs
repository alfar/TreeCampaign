using Common.Domain.Abstractions;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Events;

public sealed record OrderReceived(OrderId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}
