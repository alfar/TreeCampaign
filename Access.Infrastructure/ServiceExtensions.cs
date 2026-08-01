using Access.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Access.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddAccessRepository(this IServiceCollection services)
    {
        services.AddDbContext<AccessContext>(options =>
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddScoped<IAccessUnitOfWork, AccessContext>();
        services.AddScoped<IUserQueries, UserQueries>();

        return services;
    }
}
