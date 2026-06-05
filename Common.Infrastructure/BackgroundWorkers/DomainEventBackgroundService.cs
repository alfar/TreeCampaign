using System.Threading.Channels;
using Common.Infrastructure.BackgroundWorkers.Signals;
using Common.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.BackgroundWorkers;

public class DomainEventBackgroundService : BackgroundService
{
    private readonly ChannelReader<EventDispatchSignal> _channelReader;
    private readonly IServiceProvider _serviceProvider;

    public DomainEventBackgroundService(ChannelReader<EventDispatchSignal> channelReader, IServiceProvider serviceProvider)
    {
        _channelReader = channelReader;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            await foreach (var _ in _channelReader.ReadAllAsync(stoppingToken))
            {
                var domainEventDispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
                await domainEventDispatcher.DispatchDomainEventsAsync(stoppingToken);
            }
        }
    }
}