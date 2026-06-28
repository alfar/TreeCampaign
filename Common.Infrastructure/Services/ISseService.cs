using Common.Domain.Abstractions;

namespace Common.Infrastructure.Services;

public interface ISseService
{
    IAsyncEnumerable<string> ConnectAsync(Func<IDomainEvent, bool> filter, CancellationToken cancellationToken);
    Task BroadcastAsync(IDomainEvent @event, CancellationToken cancellationToken);
}
