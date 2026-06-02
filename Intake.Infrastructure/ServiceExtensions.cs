using Intake.Domain.Orders.Services;
using Intake.InfraStructure.Queries;
using Intake.InfraStructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Intake.InfraStructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddIntakeRepository(this IServiceCollection services)
    {
        services.AddDbContext<IntakeContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "app.db")}"));

        services.AddDbContext<IntakeProjectionContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "app.db")}"));

        services.AddScoped<IIntakeUnitOfWork, IntakeContext>();
        services.AddScoped<IAddressParser, RegexAddressParser>();
        services.AddScoped<IAddressValidationService, AddressValidationService>();
        services.AddScoped<IOrderQueries, OrderQueries>();

        return services;
    }
}
