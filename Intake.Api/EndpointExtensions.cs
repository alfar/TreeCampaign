using Intake.Api.Orders;
using Intake.Infrastructure;
using Intake.Application;
using System.Threading.Channels;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Api.JsonConverters;
using Common.InfraStructure;
using Common.Infrastructure.Services;
using Common.Infrastructure.Auth;

namespace Intake.Api;

public static class EndpointExtensions
{
    public static IServiceCollection AddIntake(this IServiceCollection services, string connectionString)
    {
        services.AddIntakeRepository(connectionString);
        services.AddIntakeServices();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new CampaignRefJsonConverter());
            options.SerializerOptions.Converters.Add(new NeighborhoodRefJsonConverter());
            options.SerializerOptions.Converters.Add(new TerritoryRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetSectionRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetRefJsonConverter());
            options.SerializerOptions.Converters.Add(new OrderIdJsonConverter());
            options.SerializerOptions.Converters.Add(new MoneyAmountJsonConverter());
            options.SerializerOptions.Converters.Add(new HouseNumberJsonConverter());
        });

        services.Configure<SseJsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new CampaignRefJsonConverter());
            options.SerializerOptions.Converters.Add(new NeighborhoodRefJsonConverter());
            options.SerializerOptions.Converters.Add(new TerritoryRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetSectionRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetRefJsonConverter());
            options.SerializerOptions.Converters.Add(new OrderIdJsonConverter());
            options.SerializerOptions.Converters.Add(new MoneyAmountJsonConverter());
            options.SerializerOptions.Converters.Add(new HouseNumberJsonConverter());
        });

        return services;
    }

    public static IEndpointRouteBuilder MapIntakeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/campaigns/{campaignId:guid}")
            .RequireAuthorization(AuthPolicies.ScoutGroupMember)
            .MapOrderEndpoints()
            .MapIntakeSseEndpoint();

        return app;
    }
}
