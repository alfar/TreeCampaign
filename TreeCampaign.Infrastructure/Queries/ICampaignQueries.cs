using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

public interface ICampaignQueries
{
    Task<IReadOnlyCollection<ProjectionContext.CampaignProjection>> GetAllByTerritoryIdAsync(TerritoryRef territoryId, CancellationToken cancellationToken);
    Task<ProjectionContext.CampaignProjection?> GetByIdAsync(CampaignId campaignId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProjectionContext.CampaignProjection>> GetCampaignsAsync(ScoutGroupRef scoutGroupId, CancellationToken cancellationToken);
}
