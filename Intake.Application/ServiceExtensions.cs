using Intake.Application.Services;
using Intake.Domain.Orders.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Intake.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddIntakeServices(this IServiceCollection services)
    {
        services.AddScoped<IAddressValidationService, AddressValidationService>();
        return services;
    }
}
