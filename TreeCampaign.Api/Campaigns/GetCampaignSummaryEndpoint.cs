using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Campaigns;

internal class GetCampaignSummaryEndpoint
{
    internal record CampaignSummary(Guid Id, Guid? TerritoryId, decimal PricePerTree);

    internal static async Task<IResult> Handle(
        Guid campaignId,
        ICampaignQueries campaignQueries,
        CancellationToken cancellationToken
    )
    {
        var campaign = await campaignQueries.GetByIdAsync(CampaignId.From(campaignId), cancellationToken);
        return campaign is null
            ? Results.NotFound()
            : Results.Ok(new CampaignSummary(
                campaign.Id.Value,
                campaign.TerritoryId?.Value,
                (campaign.PricePerTree ?? Campaign.DefaultPricePerTree).Value));
    }
}
