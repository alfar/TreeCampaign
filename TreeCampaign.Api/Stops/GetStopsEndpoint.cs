using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Repository.Queries;

namespace TreeCampaign.Api.Stops;

public class GetStopsEndpoint
{
    public static async Task<IResult> Handle(
        CampaignId campaignId,
        TeamId? teamId,
        IStopQueries stopQueries
    )
    {
        var stops =
            teamId == null
                ? await stopQueries.GetStopsAsync(campaignId)
                : await stopQueries.GetStopsByTeamIdAsync(campaignId, teamId);
        return TypedResults.Ok(stops);
    }
}
