using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using Common.Domain.Abstractions;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Services;

public class SseService : ISseService
{
    private readonly JsonSerializerOptions _jsonOptions;

    private readonly ConcurrentDictionary<Guid, (Func<IDomainEvent, bool> Filter, Channel<string> Channel)> _connections = new();

    public SseService(IOptions<SseJsonOptions> options)
    {
        _jsonOptions = options.Value.SerializerOptions;
    }

    public async IAsyncEnumerable<string> ConnectAsync(
        Func<IDomainEvent, bool> filter,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true });
        _connections[id] = (filter, channel);

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return message;
            }
        }
        finally
        {
            _connections.TryRemove(id, out _);
            channel.Writer.TryComplete();
        }
    }

    public async Task BroadcastAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        if (_connections.IsEmpty) return;

        string? payload = null;

        foreach (var (_, (filter, channel)) in _connections)
        {
            if (!filter(@event)) continue;

            payload ??= Serialize(@event);
            await channel.Writer.WriteAsync(payload, cancellationToken);
        }
    }

    private string Serialize(IDomainEvent @event)
    {
        var data = JsonSerializer.SerializeToElement(@event, @event.GetType(), _jsonOptions);
        return JsonSerializer.Serialize(new { type = @event.GetType().Name, data }, _jsonOptions);
    }
}
