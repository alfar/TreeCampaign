using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly StoredDomainEventContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDomainEventHandlerLookup _handlerLookup;

    public DomainEventDispatcher(StoredDomainEventContext context, IServiceProvider serviceProvider, IDomainEventHandlerLookup handlerLookup)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _handlerLookup = handlerLookup;
    }

    public async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var scope = _serviceProvider.CreateScope();
        foreach (var domainEvent in await _context.UnprocessedEvents.ToListAsync(cancellationToken))
        {
            try
            {
                var eventType = _handlerLookup.ResolveEventType(domainEvent.Type);

                if (eventType is not null)
                {

                    foreach (var handlerType in _handlerLookup.GetHandlerTypes(eventType))
                    {
                        var handler = (IDomainEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                        
                        var @event = System.Text.Json.JsonSerializer.Deserialize(domainEvent.Data, eventType) as IDomainEvent
                            ?? throw new InvalidOperationException($"Failed to deserialize event data for event {domainEvent.Id}.");

                        await handler.HandleAsync(@event, cancellationToken);
                    }
                }

                domainEvent.ProcessedAtUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log the exception and continue with the next event
                // You can use your preferred logging framework here
                Console.WriteLine($"Error dispatching event {domainEvent.Id}: {ex}");
            }
        }
    }
}