namespace Common.Infrastructure.Services;

public interface IDomainEventDispatcher
{
    Task DispatchDomainEventsAsync(CancellationToken cancellationToken);
}