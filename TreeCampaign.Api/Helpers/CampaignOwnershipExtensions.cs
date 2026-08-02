using Common.Infrastructure.Auth;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Helpers;

internal static class CampaignOwnershipExtensions
{
    internal static async Task<bool> IsOwnedByCurrentScoutGroupAsync(
        this ICampaignQueries campaignQueries,
        CampaignId campaignId,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken
    )
    {
        var campaign = await campaignQueries.GetByIdAsync(campaignId, cancellationToken);

        return campaign is not null && campaign.ScoutGroupId == currentUser.GetScoutGroupId();
    }
}
