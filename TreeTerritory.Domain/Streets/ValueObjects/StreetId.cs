namespace TreeTerritory.Domain.Streets.ValueObjects;

public record StreetId(Guid Value)
{
    public static bool TryParse(string? input, out StreetId streetId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            streetId = From(guid);
            return true;
        }

        streetId = From(Guid.Empty);
        return false;
    }

    public static StreetId From(Guid value) => new(value);
}