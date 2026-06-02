namespace Common.Domain.Abstractions;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> NewEvents { get; }
    void ClearEvents();
}