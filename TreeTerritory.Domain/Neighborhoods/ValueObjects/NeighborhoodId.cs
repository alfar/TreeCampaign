namespace TreeTerritory.Domain.Neighborhoods.ValueObjects;

public record NeighborhoodId(Guid Value)
{
    public static bool TryParse(string? input, out NeighborhoodId neighborhoodId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            neighborhoodId = From(guid);
            return true;
        }

        neighborhoodId = From(Guid.Empty);
        return false;
    }

    public static NeighborhoodId From(Guid value) => new NeighborhoodId(value);
}