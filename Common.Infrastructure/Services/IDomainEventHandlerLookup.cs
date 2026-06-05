using Common.InfraStructure.Abstractions;

namespace Common.Infrastructure.Services;

public interface IDomainEventHandlerLookup
{
    IEnumerable<Type> GetHandlerTypes(Type eventType);
    public Type? ResolveEventType(string typeName);
}
