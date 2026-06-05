using System.Threading.Channels;
using Common.Infrastructure.BackgroundWorkers;
using Common.Infrastructure.BackgroundWorkers.Signals;
using Common.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.InfraStructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddDomainEventServices(this IServiceCollection services)
    {
        var options = new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite
        };
        services.AddSingleton(_ => Channel.CreateBounded<EventDispatchSignal>(options));
        services.AddSingleton(sp => sp.GetRequiredService<Channel<EventDispatchSignal>>().Reader);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<EventDispatchSignal>>().Writer);
        services.AddSingleton<IDomainEventHandlerLookup, DomainEventHandlerLookup>();
        
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddHostedService<DomainEventBackgroundService>();

        services.AddDbContext<StoredDomainEventContext>(options =>
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        return services;
    }
}
