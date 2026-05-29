namespace TreeTerritory.Domain.StreetSections.ValueObjects;

public record StreetSectionId(Guid Value)
{
    public static bool TryParse(string? input, out StreetSectionId streetSectionId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            streetSectionId = From(guid);
            return true;
        }

        streetSectionId = From(Guid.Empty);
        return false;
    }

    public static StreetSectionId From(Guid value) => new StreetSectionId(value);
}
