using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;

namespace Common.Infrastructure.Services;

public class DomainEventHandlerLookup : IDomainEventHandlerLookup
{
    private readonly ILookup<Type, Type> _handlerTypes;
    private readonly Dictionary<string, Type> _typeRegistry;

    public DomainEventHandlerLookup()
    {
        // Scan all IDomainEventHandler implementations (from all assemblies your handlers live in)
        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsGenericType && typeof(IDomainEventHandler).IsAssignableFrom(t))
            .ToList();

        // Map event type → handler types
        _handlerTypes = handlerTypes
            .SelectMany(handlerType =>
                handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                    .Select(i => (eventType: i.GetGenericArguments()[0], handlerType)))
            .ToLookup(x => x.eventType, x => x.handlerType);

        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsGenericType && typeof(IDomainEvent).IsAssignableFrom(t))
            .ToList();

        // Map event type FullName → event type (for deserialization)
        _typeRegistry = eventTypes
            .DistinctBy(t => t.FullName)
            .ToDictionary(t => t.FullName!, t => t);
    }

    public IEnumerable<Type> GetHandlerTypes(Type eventType)
        => _handlerTypes[eventType];

    public Type? ResolveEventType(string typeName)
    {
        if (_typeRegistry.TryGetValue(typeName, out var type))
        {
            return type;
        }

        return null;
    }
}