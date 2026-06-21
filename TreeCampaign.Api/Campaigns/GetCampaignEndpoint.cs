using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Campaigns;

internal class GetCampaignEndpoint
{
    internal static async Task<IResult> Handle(
        Guid campaignId,
        ICampaignQueries campaignQueries,
        CancellationToken cancellationToken
    )
    {
        var campaign = await campaignQueries.GetByIdAsync(CampaignId.From(campaignId), cancellationToken);
        return campaign is null ? Results.NotFound() : Results.Ok(campaign);
    }
}
