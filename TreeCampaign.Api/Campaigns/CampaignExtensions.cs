namespace TreeCampaign.Api.Campaigns;

public static class CampaignExtensions
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/campaigns").WithTags("Campaigns");

        group.MapGet("/", GetCampaignsEndpoint.Handle);

        group.MapPost("/", CreateCampaignEndpoint.Handle);

        return app;
    }
}
