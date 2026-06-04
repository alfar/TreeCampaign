namespace Intake.Domain.ExternalReferences;

public record CampaignRef(Guid Value)
{
    public static bool TryParse(string? value, out CampaignRef campaignRef)
    {
        if (Guid.TryParse(value, out var guid))
        {
            campaignRef = new CampaignRef(guid);
            return true;
        }

        campaignRef = default!;
        return false;
    }

    public static CampaignRef From(Guid value) => new CampaignRef(value);
}
