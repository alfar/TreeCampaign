using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace TreeTerritory.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddIntakeServices(this IServiceCollection services)
    {
        return services;
    }
}
