using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;

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
}
