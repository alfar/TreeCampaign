using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;

internal class GetTeamsEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        ICampaignQueries campaignQueries,
        ITeamQueries teamQueries,
        CampaignId campaignId,
        CancellationToken cancellationToken
    )
    {
        var campaign = await campaignQueries.GetByIdAsync(campaignId, cancellationToken);

        if (campaign is null || campaign.ScoutGroupId != currentUser.GetScoutGroupId())
        {
            return Results.NotFound();
        }

        var teams = await teamQueries.GetTeamsAsync(campaignId, cancellationToken);
        return Results.Ok(teams);
    }
}
