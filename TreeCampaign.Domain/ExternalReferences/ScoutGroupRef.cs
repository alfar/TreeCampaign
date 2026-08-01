namespace TreeCampaign.Domain.ExternalReferences;

public sealed record ScoutGroupRef(Guid Value)
{
    public static bool TryParse(string? input, out ScoutGroupRef scoutGroupRef)
    {
        if (Guid.TryParse(input, out var guid))
        {
            scoutGroupRef = From(guid);
            return true;
        }

        scoutGroupRef = From(Guid.Empty);
        return false;
    }

    public static ScoutGroupRef From(Guid value) => new ScoutGroupRef(value);
}
