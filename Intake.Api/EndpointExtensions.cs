using Intake.Api.Orders;
using Intake.InfraStructure;
using Intake.Application;
using System.Threading.Channels;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Api.JsonConverters;

namespace Intake.Api;

public static class EndpointExtensions
{
    public static IServiceCollection AddIntake(this IServiceCollection services)
    {
        services.AddIntakeRepository();
        services.AddIntakeServices();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new CampaignRefJsonConverter());
            options.SerializerOptions.Converters.Add(new NeighborhoodRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetSectionRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetRefJsonConverter());
            options.SerializerOptions.Converters.Add(new OrderIdJsonConverter());
            options.SerializerOptions.Converters.Add(new MoneyAmountJsonConverter());
        });

        return services;
    }

    public static IEndpointRouteBuilder MapIntakeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/campaigns/{campaignId:guid}").MapOrderEndpoints();

        app.MapPost("/intake/testValidation", 
            async (ChannelWriter<ValidationSignalBase> signalWriter) =>
            {
                await signalWriter.WriteAsync(new EverythingValidationSignal());
            })
            .WithTags("Intake")
            .WithName("TestValidation");

        return app;
    }
}
