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

    public async Task<IReadOnlyDictionary<string, int>> GetTreeCountsByStopTypeAsync(
        CampaignId campaignId,
        CancellationToken cancellationToken
    )
    {
        // Sum in memory: Amount is a value-converted TreeCount, which EF can't aggregate server-side.
        var rows = await context
            .Stops.Where(s => EF.Property<CampaignId>(s, "CampaignId") == campaignId)
            .Select(s => new { s.StopType, s.Amount })
            .ToListAsync(cancellationToken);

        return rows.GroupBy(r => r.StopType).ToDictionary(g => g.Key, g => g.Sum(r => r.Amount.Value));
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
