using Intake.Domain.ExternalReferences;

namespace Intake.Domain.Orders.Services;

public interface ICampaignOwnershipService
{
    Task<bool> IsOwnedByScoutGroupAsync(CampaignRef campaignId, ScoutGroupRef scoutGroupId, CancellationToken cancellationToken = default);
}
