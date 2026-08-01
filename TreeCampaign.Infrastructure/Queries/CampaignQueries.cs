using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Infrastructure.Queries;

public class CampaignQueries(ProjectionContext context) : ICampaignQueries
{
    public async Task<
        IReadOnlyCollection<ProjectionContext.CampaignProjection>
    > GetCampaignsAsync(ScoutGroupRef scoutGroupId, CancellationToken cancellationToken)
    {
        return await context.Campaigns.Where(c => c.ScoutGroupId == scoutGroupId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProjectionContext.CampaignProjection>> GetAllByTerritoryIdAsync(TerritoryRef territoryId, CancellationToken cancellationToken)
    {
        return await context.Campaigns.Where(c => c.TerritoryId == territoryId).ToListAsync(cancellationToken);
    }

    public async Task<ProjectionContext.CampaignProjection?> GetByIdAsync(CampaignId campaignId, CancellationToken cancellationToken)
    {
        return await context.Campaigns.Where(c => c.Id == campaignId).FirstOrDefaultAsync(cancellationToken);        
    }
}
