using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Campaigns;

internal class GetCampaignEndpoint
{
    internal static async Task<IResult> Handle(
        Guid campaignId,
        ICurrentUserAccessor currentUser,
        ICampaignQueries campaignQueries,
        CancellationToken cancellationToken
    )
    {
        var campaign = await campaignQueries.GetByIdAsync(CampaignId.From(campaignId), cancellationToken);
        return campaign is null || campaign.ScoutGroupId != currentUser.GetScoutGroupId() ? Results.NotFound() : Results.Ok(campaign);
    }
}
