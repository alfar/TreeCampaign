using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure.Queries;

namespace TreeCampaign.Api.Stops;

public class GetStopsEndpoint
{
    public static async Task<IResult> Handle(
        CampaignId campaignId,
        TeamId? teamId,
        ICurrentUserAccessor currentUser,
        ICampaignQueries campaignQueries,
        IStopQueries stopQueries,
        CancellationToken cancellationToken
    )
    {
        if (teamId is null)
        {
            if (!await campaignQueries.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(await stopQueries.GetStopsAsync(campaignId));
        }

        return TypedResults.Ok(await stopQueries.GetStopsByTeamIdAsync(campaignId, teamId));
    }
}
