using Common.Infrastructure.Auth;

namespace TreeCampaign.Api.Campaigns;

public static class CampaignExtensions
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/campaigns").WithTags("Campaigns").RequireAuthorization(AuthPolicies.ScoutGroupMember);

        group.MapGet("/", GetCampaignsEndpoint.Handle);
        group.MapGet("/{campaignId:guid}", GetCampaignEndpoint.Handle);

        group.MapPost("/", CreateCampaignEndpoint.Handle);

        group.MapPut("/{campaignId:guid}", UpdateCampaignEndpoint.Handle);

        // Unauthenticated: the team detail route needs this to show the pickup form for scouts in the field who aren't logged in.
        app.MapGroup("/campaigns")
            .WithTags("Campaigns")
            .MapGet("/{campaignId:guid}/summary", GetCampaignSummaryEndpoint.Handle);

        return app;
    }
}
