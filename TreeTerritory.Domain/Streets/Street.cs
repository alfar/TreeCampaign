using Common.Domain.Abstractions;
using TreeTerritory.Domain.Streets.Events;
using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.Domain.Streets;

public class Street : IHasDomainEvents
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

    public required StreetId Id { get; init; }
    public string Name { get; private set; } = string.Empty;

    public ZipCode ZipCode { get; private set; } = ZipCode.Empty;

    public static Street Create(string name, ZipCode zipCode)
    {
        var result = new Street
        {
            Id = StreetId.From(Guid.NewGuid()),
            Name = name,
            ZipCode = zipCode
        };

        result.Raise(new StreetCreated(result.Id));

        return result;
    }

    private Street()
    { }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateZipCode(ZipCode zipCode)
    {
        ZipCode = zipCode;
    }
}
