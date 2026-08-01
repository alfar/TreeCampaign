using Common.Infrastructure.Auth;

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
