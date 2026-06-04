using Intake.Api.Orders;
using Intake.InfraStructure;
using Intake.Application;

namespace Intake.Api;

public static class EndpointExtensions
{
    public static IServiceCollection AddIntake(this IServiceCollection services)
    {
        services.AddIntakeRepository();
        services.AddIntakeServices();
        return services;
    }

    public static IEndpointRouteBuilder MapIntakeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/campaigns/{campaignId:guid}").MapOrderEndpoints();
        return app;
    }
}
