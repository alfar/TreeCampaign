using Common.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using TreeTerritory.Application.Services;
using TreeTerritory.Domain.Neighborhoods.Services;

namespace TreeTerritory.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddTreeTerritoryServices(this IServiceCollection services)
    {
        services.AddScoped<ICsvStreetSectionParser, CsvStreetSectionParser>();
        services.AddScoped<IStreetSectionImportService, StreetSectionImportService>();

        services.AddHttpClient<IAddressLookupClient, AdressevaelgerClient>();

        return services;
    }
}
