using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Infrastructure;

namespace Intake.Application.Services;

public class CampaignOwnershipService(TreeCampaignContext campaignContext) : ICampaignOwnershipService
{
    public async Task<bool> IsOwnedByScoutGroupAsync(CampaignRef campaignId, ScoutGroupRef scoutGroupId, CancellationToken cancellationToken = default)
    {
        var campaign = await campaignContext.CollectionCampaigns
            .FirstOrDefaultAsync(c => c.Id == CampaignId.From(campaignId.Value), cancellationToken);

        return campaign is not null && campaign.ScoutGroupId.Value == scoutGroupId.Value;
    }
}
