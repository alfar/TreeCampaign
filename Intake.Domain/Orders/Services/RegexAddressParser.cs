using System.Text.RegularExpressions;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Services;

public class RegexAddressParser : IAddressParser
{
    // Matches Danish addresses: <street name> <number>[<letter>][, <zip>[ <city>]]
    // Street names may contain Æ Ø Å and hyphens (e.g. "H.C. Andersens Vej", "Nørre-Allé")
    private static readonly Regex AddressPattern = new(
        @"(?<street>[A-Za-zÆæØøÅå][A-Za-zÆæØøÅå0-9\s\-\.]+)" +
        @"\s+(?<number>\d+)\s*(?<letter>[A-Za-zÆæØøÅå])?" +
        @"(?:\s*,\s*(?<zip>\d{4})(?:\s+(?<city>[A-Za-zÆæØøÅå\s]+))?)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline
    );

    public ParsedAddress? TryParse(string message)
    {
        var match = AddressPattern.Match(message);
        if (!match.Success)
            return null;

        var street = match.Groups["street"].Value.Trim();
        if (string.IsNullOrWhiteSpace(street))
            return null;

        var number = match.Groups["number"].Value;
        var letter = match.Groups["letter"].Value;
        var houseNumber = string.IsNullOrEmpty(letter) ? number : $"{number}{letter}";
        var zipCode = match.Groups["zip"].Success ? match.Groups["zip"].Value : null;
        var city = match.Groups["city"].Success ? match.Groups["city"].Value.Trim() : null;

        return new ParsedAddress(street, houseNumber, zipCode, string.IsNullOrEmpty(city) ? null : city);
    }
}
