using System.Text.RegularExpressions;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Services;

public class RegexAddressParser : IAddressParser
{
    // Matches Danish addresses: <street name> <number>[<letter>]
    // Street names may contain any Unicode letter (\p{L}, e.g. Æ Ø Å é ü ö) and hyphens/dots
    // (e.g. "H.C. Andersens Vej", "Nørre-Allé").
    // Anchored to a word boundary (not end-of-string) so trailing free text - order notes,
    // floor/side ("1.th"), neighborhood names, emoji, etc. - doesn't prevent a match.
    // A single letter directly after the number is always captured as a suite letter (e.g. "45 a",
    // "6a"). This is ambiguous with a following one-letter word ("39 i Sydbyen" -> house number
    // "39i") but that's acceptable here - a bogus house number like that fails address validation
    // downstream rather than silently producing a wrong-but-plausible result.
    private static readonly Regex AddressPattern = new(
        @"(?<street>\p{L}[\p{L}\-\.]*(?:\s+\p{L}[\p{L}\-\.]*)*)" +
        @"\s+(?<number>\d+)(?:\s*(?<letter>\p{L})\b)?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    // Leading pickup notes that precede the street rather than being part of it (e.g. "Afh. Vejlbovej 46").
    private static readonly Regex LeadingNoisePattern = new(
        @"^\s*(?:afh(?:ent(?:es)?)?\.?)\s+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    // Danish zip codes are 4 digits; matched as a standalone token anywhere in the message.
    private static readonly Regex ZipPattern = new(@"\b\d{4}\b", RegexOptions.Compiled);

    public ParsedAddress? TryParse(string message)
    {
        var search = LeadingNoisePattern.Replace(message, "", 1);
        var match = AddressPattern.Match(search);
        if (!match.Success)
            return null;

        var street = match.Groups["street"].Value.Trim();
        if (string.IsNullOrWhiteSpace(street))
            return null;

        var number = match.Groups["number"].Value;
        var letter = match.Groups["letter"].Value;
        var houseNumber = string.IsNullOrEmpty(letter) ? number : $"{number}{letter}";

        var zipMatch = ZipPattern.Match(message);
        var zipCode = zipMatch.Success ? zipMatch.Value : null;

        return new ParsedAddress(street, houseNumber, zipCode, null);
    }
}
