using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Infrastructure.Queries;

public class StopQueries(ProjectionContext context) : IStopQueries
{
    public async Task<IReadOnlyCollection<ProjectionContext.StopProjection>> GetStopsByTeamIdAsync(
        CampaignId campaignId,
        TeamId teamId
    )
    {
        return
        [
            .. context.Stops.Where(s =>
                EF.Property<CampaignId>(s, "CampaignId") == campaignId
                && EF.Property<TeamId?>(s, "AssignedTeamId") == teamId
            ),
        ];
    }

    public async Task<IReadOnlyCollection<ProjectionContext.StopProjection>> GetStopsAsync(
        CampaignId campaignId
    )
    {
        return
        [
            .. context.Stops.Where(s => EF.Property<CampaignId>(s, "CampaignId") == campaignId),
        ];
    }
}
