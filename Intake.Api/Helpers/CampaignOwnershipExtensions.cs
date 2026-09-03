using Common.Infrastructure.Auth;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;

namespace Intake.Api.Helpers;

internal static class CampaignOwnershipExtensions
{
    internal static async Task<bool> IsOwnedByCurrentScoutGroupAsync(
        this ICampaignOwnershipService campaignOwnershipService,
        CampaignRef campaignId,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken
    )
    {
        return await campaignOwnershipService.IsOwnedByScoutGroupAsync(campaignId, currentUser.GetScoutGroupId(), cancellationToken);
    }
}
