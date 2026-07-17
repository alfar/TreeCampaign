using Common.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TreeCampaign.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddTreeCampaignApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<PickupRequestService>();
        services.AddHttpClient<IAddressLookupClient, AdressevaelgerClient>();
        return services;
    }
}
