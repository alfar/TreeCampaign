using TreeCampaign.Api.Campaigns;
using TreeCampaign.Api.Stops;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapTreeCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCampaignEndpoints().MapTeamEndpoints().MapStopEndpoints();

        return app;
    }
}