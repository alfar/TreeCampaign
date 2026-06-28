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
    private readonly ISseService _sseService;

    public DomainEventDispatcher(StoredDomainEventContext context, IServiceProvider serviceProvider, IDomainEventHandlerLookup handlerLookup, ISseService sseService)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _handlerLookup = handlerLookup;
        _sseService = sseService;
    }

    public async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var scope = _serviceProvider.CreateScope();
        foreach (var storedEvent in await _context.UnprocessedEvents.ToListAsync(cancellationToken))
        {
            try
            {
                var eventType = _handlerLookup.ResolveEventType(storedEvent.Type);

                if (eventType is not null)
                {
                    var @event = System.Text.Json.JsonSerializer.Deserialize(storedEvent.Data, eventType) as IDomainEvent
                        ?? throw new InvalidOperationException($"Failed to deserialize event data for event {storedEvent.Id}.");

                    await _sseService.BroadcastAsync(@event, cancellationToken);

                    foreach (var handlerType in _handlerLookup.GetHandlerTypes(eventType))
                    {
                        var handler = (IDomainEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                        await handler.HandleAsync(@event, cancellationToken);
                    }
                }

                storedEvent.ProcessedAtUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dispatching event {storedEvent.Id}: {ex}");
            }
        }
    }
}