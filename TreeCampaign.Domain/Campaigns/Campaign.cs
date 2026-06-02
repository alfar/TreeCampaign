using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Domain.Campaigns;

public sealed class Campaign
{
    public required CampaignId Id { get; init; }
    public required CampaignSeason Season { get; init; }
    public TerritoryRef? TerritoryId { get; private set; }

    private Campaign() { }

    public static Campaign Create(CampaignSeason season, TerritoryRef? territoryId = null)
    {
        return new Campaign { Id = CampaignId.From(Guid.NewGuid()), Season = season, TerritoryId = territoryId };
    }

    public void SetTerritory(TerritoryRef territoryId)
    {
        TerritoryId = territoryId;
    }
}
