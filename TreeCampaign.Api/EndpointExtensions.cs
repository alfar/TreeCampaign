using Common.Infrastructure.Services;
using TreeCampaign.Api.Campaigns;
using TreeCampaign.Api.Stops;
using TreeCampaign.Application;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapTreeCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapCampaignEndpoints()
            .MapGroup("/{campaignId}")
            .MapStopEndpoints()
            .MapTeamEndpoints()
            .MapCampaignSseEndpoint();

        return app;
    }

    public static IServiceCollection AddTreeCampaign(this IServiceCollection services)
    {
        services.AddTreeCampaignRepository();
        services.AddTreeCampaignApplicationServices();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new TreeCountJsonConverter());
            options.SerializerOptions.Converters.Add(new StopIdJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamIdJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamMemberIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CollectionCampaignIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CampaignSeasonJsonConverter());
            options.SerializerOptions.Converters.Add(new ReasonTextJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamNameJsonConverter());
            options.SerializerOptions.Converters.Add(new TerritoryRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetSectionRefJsonConverter());
            options.SerializerOptions.Converters.Add(new ScoutGroupRefJsonConverter());
        });

        services.Configure<SseJsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new TreeCountJsonConverter());
            options.SerializerOptions.Converters.Add(new StopIdJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamIdJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamMemberIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CollectionCampaignIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CampaignSeasonJsonConverter());
            options.SerializerOptions.Converters.Add(new ReasonTextJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamNameJsonConverter());
            options.SerializerOptions.Converters.Add(new TerritoryRefJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetSectionRefJsonConverter());
            options.SerializerOptions.Converters.Add(new ScoutGroupRefJsonConverter());
        });

        return services;
    }
}