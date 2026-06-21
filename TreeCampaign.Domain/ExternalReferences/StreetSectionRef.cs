namespace TreeCampaign.Domain.ExternalReferences;

public record StreetSectionRef(Guid Value)
{
    public static StreetSectionRef From(Guid value) => new StreetSectionRef(value);
}
