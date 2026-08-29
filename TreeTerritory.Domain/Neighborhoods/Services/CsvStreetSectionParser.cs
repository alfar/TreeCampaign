using System.Globalization;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Domain.Neighborhoods.Services;

public class CsvStreetSectionParser : ICsvStreetSectionParser
{
    private const string StreetNameColumn = "Vejnavn";
    private const string EvenFromColumn = "Lige husnummer fra";
    private const string EvenToColumn = "Lige husnummer til";
    private const string OddFromColumn = "Ulige husnummer fra";
    private const string OddToColumn = "Ulige husnummer til";
    private const string TrailerTypeColumn = "Trailertype";
    private const string NeighborhoodColumn = "Kvarter";
    private const string SortOrderColumn = "Rækkefølge";
    private const string DirectionColumn = "Sortering";

    public IReadOnlyList<StreetSectionParsingResult> Parse(string csvContent)
    {
        var lines = csvContent.ReplaceLineEndings("\n").Split('\n');
        if (lines.Length < 2)
            return [];

        var columnIndex = BuildColumnIndex(lines[0]);

        var results = new List<StreetSectionParsingResult>();
        for (var i = 1; i < lines.Length; i++)
        {
            var lineNumber = i + 1;
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var row = line.Split(';');
            if (row.Length < columnIndex.Count)
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, "Uventet antal kolonner"));
                continue;
            }

            var streetName = row[columnIndex[StreetNameColumn]].Trim();
            if (streetName.Length == 0)
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, "Mangler vejnavn"));
                continue;
            }

            var neighborhoodName = row[columnIndex[NeighborhoodColumn]].Trim();
            if (neighborhoodName.Length == 0)
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, "Mangler kvarter"));
                continue;
            }

            if (!TryParseHouseNumber(row[columnIndex[EvenFromColumn]], out var evenFrom))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Kunne ikke læse '{EvenFromColumn}': '{row[columnIndex[EvenFromColumn]]}'"));
                continue;
            }

            if (!TryParseHouseNumber(row[columnIndex[EvenToColumn]], out var evenTo))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Kunne ikke læse '{EvenToColumn}': '{row[columnIndex[EvenToColumn]]}'"));
                continue;
            }

            if (!TryParseHouseNumber(row[columnIndex[OddFromColumn]], out var oddFrom))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Kunne ikke læse '{OddFromColumn}': '{row[columnIndex[OddFromColumn]]}'"));
                continue;
            }

            if (!TryParseHouseNumber(row[columnIndex[OddToColumn]], out var oddTo))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Kunne ikke læse '{OddToColumn}': '{row[columnIndex[OddToColumn]]}'"));
                continue;
            }

            if (!TryParseTrailerSize(row[columnIndex[TrailerTypeColumn]], out var trailerSize))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Ukendt trailertype '{row[columnIndex[TrailerTypeColumn]]}'"));
                continue;
            }

            var sortOrderText = row[columnIndex[SortOrderColumn]].Trim();
            if (!int.TryParse(sortOrderText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sortOrder))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Kunne ikke læse rækkefølge '{sortOrderText}'"));
                continue;
            }

            if (!TryParseDirection(row[columnIndex[DirectionColumn]], out var direction))
            {
                results.Add(new StreetSectionParsingFailed(lineNumber, $"Ukendt sortering '{row[columnIndex[DirectionColumn]]}'"));
                continue;
            }

            results.Add(new ParsedStreetSection(
                lineNumber,
                streetName,
                evenFrom,
                evenTo,
                oddFrom,
                oddTo,
                trailerSize,
                neighborhoodName,
                sortOrder,
                direction));
        }

        return results;
    }

    private static bool TryParseHouseNumber(string input, out HouseNumber? houseNumber)
    {
        var trimmed = input.Trim();
        if (trimmed.Length == 0)
        {
            houseNumber = null;
            return true;
        }

        if (!HouseNumber.TryParse(trimmed, out var parsed))
        {
            houseNumber = null;
            return false;
        }

        houseNumber = parsed;
        return true;
    }

    private static bool TryParseTrailerSize(string input, out TrailerSize trailerSize)
    {
        var trimmed = input.Trim();
        switch (trimmed.ToLowerInvariant())
        {
            case "":
            case "boogie":
                trailerSize = TrailerSize.Boogie;
                return true;
            case "lille":
                trailerSize = TrailerSize.Small;
                return true;
            case "stor":
                trailerSize = TrailerSize.Large;
                return true;
            default:
                trailerSize = default;
                return false;
        }
    }

    private static bool TryParseDirection(string input, out Direction direction)
    {
        switch (input.Trim().ToLowerInvariant())
        {
            case "stigende":
                direction = Direction.Ascending;
                return true;
            case "faldende":
                direction = Direction.Descending;
                return true;
            default:
                direction = default;
                return false;
        }
    }

    private static Dictionary<string, int> BuildColumnIndex(string headerLine)
    {
        var headers = headerLine.Split(';');
        var columnIndex = new Dictionary<string, int>();
        for (var i = 0; i < headers.Length; i++)
        {
            columnIndex[headers[i].Trim()] = i;
        }

        foreach (var required in new[]
        {
            StreetNameColumn, EvenFromColumn, EvenToColumn, OddFromColumn, OddToColumn,
            TrailerTypeColumn, NeighborhoodColumn, SortOrderColumn, DirectionColumn
        })
        {
            if (!columnIndex.ContainsKey(required))
                throw new InvalidOperationException($"CSV-filen mangler kolonnen '{required}'");
        }

        return columnIndex;
    }
}
