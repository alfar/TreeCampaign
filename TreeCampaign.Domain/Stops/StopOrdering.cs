using TreeCampaign.Domain.Stops.ValueObjects;

namespace TreeCampaign.Domain.Stops;

/// <summary>
/// Route position of a street section, supplied by the Territory context as plain data
/// (no compile-time dependency on the Territory domain).
/// </summary>
public record SectionOrder(int SortOrder, bool Descending);

/// <summary>
/// Orders stops along the route: by street section sort order, then by house number
/// following the section's direction of travel. Stops whose section is unknown go last.
/// </summary>
public static class StopOrdering
{
    public static IComparer<Address> Comparer(IReadOnlyDictionary<Guid, SectionOrder> sections) =>
        Comparer<Address>.Create((a, b) => Compare(a, b, sections));

    public static int Compare(Address a, Address b, IReadOnlyDictionary<Guid, SectionOrder> sections)
    {
        var hasA = sections.TryGetValue(a.StreetSectionId.Value, out var sA);
        var hasB = sections.TryGetValue(b.StreetSectionId.Value, out var sB);
        if (!hasA && !hasB) return 0;
        if (!hasA) return 1;
        if (!hasB) return -1;
        if (sA!.SortOrder != sB!.SortOrder) return sA.SortOrder.CompareTo(sB.SortOrder);

        var hA = ParseHouseNumber(a.DisplayName);
        var hB = ParseHouseNumber(b.DisplayName);
        return sA.Descending ? hB.CompareTo(hA) : hA.CompareTo(hB);
    }

    // DisplayName looks like "Storgade 12B, 8600 Silkeborg": take the last token before the comma
    // and read its leading digits (mirrors the dispatch screen's parsing).
    public static int ParseHouseNumber(string displayName)
    {
        var street = displayName.Split(',')[0];
        var lastToken = street.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";
        var digits = new string(lastToken.TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : 0;
    }
}
