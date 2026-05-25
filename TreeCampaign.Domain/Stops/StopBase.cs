using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Domain.Stops;

public abstract class StopBase
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

    public required StopId Id { get; init; }
    public required CampaignId CampaignId { get; init; }
    public required Address Address { get; init; }
    public required TreeCount Amount { get; init; }

    protected StopBase() { }
}
