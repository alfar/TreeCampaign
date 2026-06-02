namespace Intake.Domain.ExternalReferences;

public record CampaignRef(Guid Value)
{
    public static CampaignRef From(Guid value) => new CampaignRef(value);
}
