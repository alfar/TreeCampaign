using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;

namespace TreeCampaign.Api.Campaigns;

internal class GetCampaignsEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        ICampaignQueries campaignQueries,
        CancellationToken cancellationToken
    )
    {
        var campaigns = await campaignQueries.GetCampaignsAsync(currentUser.GetScoutGroupId(), cancellationToken);
        return Results.Ok(campaigns);
    }
}
