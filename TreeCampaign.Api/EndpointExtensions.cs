using TreeCampaign.Api.Campaigns;
using TreeCampaign.Api.Stops;
using TreeCampaign.InfraStructure;

namespace TreeCampaign.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapTreeCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapCampaignEndpoints()
            .MapGroup("/{campaignId}")
            .MapStopEndpoints()
            .MapTeamEndpoints();

        return app;
    }

    public static IServiceCollection AddTreeCampaign(this IServiceCollection services)
    {
        services.AddTreeCampaignRepository();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new TreeCountJsonConverter());
            options.SerializerOptions.Converters.Add(new StopIdJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CollectionCampaignIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CampaignSeasonJsonConverter());
            options.SerializerOptions.Converters.Add(new ReasonTextJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamNameJsonConverter());
        });

        return services;
    }
}