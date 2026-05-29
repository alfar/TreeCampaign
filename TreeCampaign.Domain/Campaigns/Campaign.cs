using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Domain.Campaigns;

public sealed class Campaign
{
    public required CampaignId Id { get; init; }
    public required CampaignSeason Season { get; init; }

    private Campaign() { }

    public static Campaign Create(CampaignSeason season)
    {
        return new Campaign { Id = CampaignId.From(Guid.NewGuid()), Season = season };
    }
}
