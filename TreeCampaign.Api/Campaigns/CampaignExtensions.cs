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

        // Unauthenticated: the team detail route needs these for scouts in the field who aren't logged in.
        var publicGroup = app.MapGroup("/campaigns").WithTags("Campaigns");
        // Pickup form.
        publicGroup.MapGet("/{campaignId:guid}/summary", GetCampaignSummaryEndpoint.Handle);
        // Progress pie on the team info tab (aggregate counts only).
        publicGroup.MapGet("/{campaignId:guid}/progress", GetCampaignProgressEndpoint.Handle);

        return app;
    }
}
