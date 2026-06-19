using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Domain.Campaigns;

public sealed class Campaign
{
    public required CampaignId Id { get; init; }
    public CampaignSeason Season { get; private set; }
    public TerritoryRef? TerritoryId { get; private set; }

    private Campaign() { Season = default!; }

    public static Campaign Create(CampaignSeason season, TerritoryRef? territoryId = null)
    {
        return new Campaign { Id = CampaignId.From(Guid.NewGuid()), Season = season, TerritoryId = territoryId };
    }

    public void SetSeason(CampaignSeason season)
    {
        Season = season;
    }

    public void SetTerritory(TerritoryRef territoryId)
    {
        TerritoryId = territoryId;
    }
}
