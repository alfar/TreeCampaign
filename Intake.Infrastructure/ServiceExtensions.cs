using Intake.Domain.Orders.Services;
using Intake.Domain.Services;
using Intake.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Intake.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddIntakeRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IntakeContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDbContext<IntakeProjectionContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IIntakeUnitOfWork>(sp => sp.GetRequiredService<IntakeContext>());
        services.AddScoped<IAddressParser, RegexAddressParser>();
        services.AddScoped<IOrderQueries, OrderQueries>();

        return services;
    }
}
