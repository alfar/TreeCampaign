using Microsoft.EntityFrameworkCore;

namespace TreeCampaign.InfraStructure.Queries;

public class CampaignQueries(ProjectionContext context) : ICampaignQueries
{
    async Task<
        IReadOnlyCollection<ProjectionContext.CampaignProjection>
    > ICampaignQueries.GetCampaignsAsync(CancellationToken cancellationToken)
    {
        return await context.Campaigns.ToListAsync(cancellationToken);
    }
}
