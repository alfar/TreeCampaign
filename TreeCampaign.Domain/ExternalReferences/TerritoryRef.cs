namespace TreeCampaign.Domain.ExternalReferences;

public sealed record TerritoryRef(Guid Value)
{
    public static bool TryParse(string? input, out TerritoryRef territoryRef)
    {
        if (Guid.TryParse(input, out var guid))
        {
            territoryRef = From(guid);
            return true;
        }

        territoryRef = From(Guid.Empty);
        return false;
    }

    public static TerritoryRef From(Guid value) => new TerritoryRef(value);
}
