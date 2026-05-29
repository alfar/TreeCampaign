namespace TreeTerritory.Domain.Territories.ValueObjects;

public record TerritoryId(Guid Value)
{
    public static bool TryParse(string? input, out TerritoryId territoryId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            territoryId = From(guid);
            return true;
        }

        territoryId = From(Guid.Empty);
        return false;
    }

    public static TerritoryId From(Guid value) => new TerritoryId(value);
}