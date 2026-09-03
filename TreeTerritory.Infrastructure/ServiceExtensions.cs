using Common.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddTreeTerritoryRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TreeTerritoryContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ITreeTerritoryUnitOfWork>(sp => sp.GetRequiredService<TreeTerritoryContext>());
        services.AddScoped<INeighborhoodQueries, NeighborhoodQueries>();
        services.AddScoped<ITerritoryQueries, TerritoryQueries>();
        services.AddScoped<IStreetQueries, StreetQueries>();
        services.AddScoped<IStreetSectionQueries, StreetSectionQueries>();

        return services;
    }
}
