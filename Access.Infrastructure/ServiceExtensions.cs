using Access.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Access.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddAccessRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AccessContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IAccessUnitOfWork>(sp => sp.GetRequiredService<AccessContext>());
        services.AddScoped<IUserQueries, UserQueries>();

        return services;
    }
}
