using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Infrastructure.Queries;

public class TeamQueries(ProjectionContext context) : ITeamQueries
{
    async Task<IReadOnlyCollection<ProjectionContext.TeamProjection>> ITeamQueries.GetTeamsAsync(
        CampaignId campaignId,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Teams
            .Include(t => t.Members)
            .Where(t => EF.Property<CampaignId>(t, "CampaignId") == campaignId)
            .ToListAsync();
    }

    async Task<int> ITeamQueries.GetTotalExtraTreesAsync(
        CampaignId campaignId,
        CancellationToken cancellationToken
    )
    {
        var totals = await context
            .Teams.Where(t => EF.Property<CampaignId>(t, "CampaignId") == campaignId)
            .Select(t => t.TotalExtraTrees)
            .ToListAsync(cancellationToken);

        return totals.Sum(t => t.Value);
    }

    async Task<ProjectionContext.TeamProjection?> ITeamQueries.GetTeamAsync(
        TeamId teamId,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
    }
}
