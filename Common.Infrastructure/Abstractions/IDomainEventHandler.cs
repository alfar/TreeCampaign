using Common.Domain.Abstractions;

namespace Common.InfraStructure.Abstractions;

public interface IDomainEventHandler
{
    Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventHandler<T> : IDomainEventHandler;
