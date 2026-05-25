namespace TreeCampaign.Domain.Campaigns.ValueObjects;

public sealed record CampaignId(Guid Value)
{
    public static bool TryParse(string? input, out CampaignId campaignId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            campaignId = From(guid);
            return true;
        }

        campaignId = From(Guid.Empty);
        return false;
    }

    public static CampaignId From(Guid value) => new CampaignId(value);
}
