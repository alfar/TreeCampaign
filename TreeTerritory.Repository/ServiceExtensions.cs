using Common.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TreeTerritory.Repository;

public static class ServiceExtensions
{
    public static IServiceCollection AddTreeTerritoryRepository(this IServiceCollection services)
    {
        services.AddDbContext<TreeTerritoryContext>(options =>
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddScoped<IUnitOfWork, TreeTerritoryContext>();

        return services;
    }
}
