using Common.Infrastructure.Auth;
using Common.Infrastructure.Services;
using TreeCampaign.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Api.Campaigns;

public static class CampaignSseEndpoint
{
    public static IEndpointRouteBuilder MapCampaignSseEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/events", Handle).WithTags("Campaigns");
        return app;
    }

    private static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        CampaignId campaignId,
        ISseService sseService,
        ICampaignQueries campaignQueries,
        CancellationToken cancellationToken)
    {
        var campaign = await campaignQueries.GetByIdAsync(campaignId, cancellationToken);

        if (campaign is null || campaign.ScoutGroupId != ScoutGroupRef.From(currentUser.ScoutGroupId!.Value))
        {
            return Results.NotFound();
        }

        var stream = sseService.ConnectAsync(
                 @event => @event is ITreeCampaignEvent tce && tce.CampaignId == campaignId,
                 cancellationToken);

        return Results.ServerSentEvents(stream, eventType: "campaign-update");
    }
}
