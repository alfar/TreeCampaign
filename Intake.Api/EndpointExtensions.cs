using Intake.Repository;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Intake.Api;

public static class EndpointExtensions
{
    public static IServiceCollection AddIntake(this IServiceCollection services)
    {
        services.AddIntakeRepository();
        return services;
    }

    public static IEndpointRouteBuilder MapIntakeEndpoints(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
