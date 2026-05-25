namespace TreeCampaign.Domain.Campaigns.ValueObjects;

public sealed record CampaignSeason(int Year)
{
    public static CampaignSeason From(int year) => new CampaignSeason(year);
}
