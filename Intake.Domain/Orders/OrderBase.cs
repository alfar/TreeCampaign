using Common.Domain.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public abstract class OrderBase
{
    private readonly List<IDomainEvent> _newEvents = new();
    public IReadOnlyCollection<IDomainEvent> NewEvents => _newEvents.AsReadOnly();

    protected void Raise(IDomainEvent @event)
    {
        _newEvents.Add(@event);
    }

    public void ClearEvents()
    {
        _newEvents.Clear();
    }

    public required OrderId Id { get; init; }

    public required CampaignRef CampaignId { get; init; }

    public required Sender Sender { get; init; }

    public required MoneyAmount Amount { get; init; }

    public required DateTimeOffset OrderDate { get; init; }

    public required string Message { get; init; }

    protected OrderBase() { }
}
