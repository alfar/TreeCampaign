using Common.Infrastructure.Services;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.Services;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Application.Services;

public record StreetSectionImportFailure(int LineNumber, string Reason);

public record StreetSectionImportSummary(
    int ImportedCount,
    int SkippedExistingCount,
    IReadOnlyList<StreetSectionImportFailure> Failures);

public interface IStreetSectionImportService
{
    Task<StreetSectionImportSummary> ImportAsync(TerritoryId territoryId, string csvContent, CancellationToken cancellationToken);
}

public class StreetSectionImportService(
    ICsvStreetSectionParser parser,
    ITreeTerritoryUnitOfWork unitOfWork,
    ITerritoryQueries territoryQueries,
    INeighborhoodQueries neighborhoodQueries,
    IStreetQueries streetQueries,
    IAddressLookupClient addressLookupClient) : IStreetSectionImportService
{
    public async Task<StreetSectionImportSummary> ImportAsync(TerritoryId territoryId, string csvContent, CancellationToken cancellationToken)
    {
        var territory = await territoryQueries.GetByIdAsync(territoryId, cancellationToken)
            ?? throw new InvalidOperationException($"Territory '{territoryId}' not found.");

        if (territory.DefaultZipCode is null)
            throw new InvalidOperationException("Territoriet har intet standard-postnummer sat. Sæt et postnummer på territoriet før import.");

        var zipCode = territory.DefaultZipCode;

        var parsed = parser.Parse(csvContent);

        var rows = parsed.OfType<ParsedStreetSection>().ToList();
        var failures = parsed.OfType<StreetSectionParsingFailed>()
            .Select(f => new StreetSectionImportFailure(f.LineNumber, f.Reason))
            .ToList();

        var neighborhoodRepository = unitOfWork.GetRepository<Neighborhood, NeighborhoodId>();
        var streetRepository = unitOfWork.GetRepository<Street, StreetId>();

        var neighborhoodsByName = new Dictionary<string, Neighborhood>(StringComparer.OrdinalIgnoreCase);
        var streetsByName = new Dictionary<string, Street?>(StringComparer.OrdinalIgnoreCase);

        var importedCount = 0;
        var skippedExistingCount = 0;

        foreach (var row in rows)
        {
            if (!streetsByName.TryGetValue(row.StreetName, out var street))
            {
                street = await streetQueries.GetByNameAndZipCodeAsync(row.StreetName, zipCode, cancellationToken);
                if (street is null)
                {
                    var matches = await addressLookupClient.SearchStreets(row.StreetName, zipCode.Value);
                    var isVerified = matches.Any(m => string.Equals(m.StreetName, row.StreetName, StringComparison.OrdinalIgnoreCase));
                    if (isVerified)
                    {
                        street = Street.Create(row.StreetName, zipCode);
                        streetRepository.Add(street);
                    }
                }

                streetsByName[row.StreetName] = street;
            }

            if (street is null)
            {
                failures.Add(new StreetSectionImportFailure(row.LineNumber, $"Vejnavnet '{row.StreetName}' kunne ikke genkendes"));
                continue;
            }

            if (!neighborhoodsByName.TryGetValue(row.NeighborhoodName, out var neighborhood))
            {
                neighborhood = await neighborhoodQueries.GetByNameAsync(territoryId, row.NeighborhoodName, cancellationToken);
                if (neighborhood is null)
                {
                    neighborhood = Neighborhood.Create(territoryId, row.NeighborhoodName);
                    neighborhoodRepository.Add(neighborhood);
                }

                neighborhoodsByName[row.NeighborhoodName] = neighborhood;
            }

            var alreadyExists = neighborhood.StreetSections.Any(section =>
                section.StreetId == street.Id &&
                section.EvenStartHouseNumber == row.EvenStartHouseNumber &&
                section.EvenEndHouseNumber == row.EvenEndHouseNumber &&
                section.OddStartHouseNumber == row.OddStartHouseNumber &&
                section.OddEndHouseNumber == row.OddEndHouseNumber);

            if (alreadyExists)
            {
                skippedExistingCount++;
                continue;
            }

            neighborhood.AddStreetSection(
                street.Id,
                row.EvenStartHouseNumber,
                row.EvenEndHouseNumber,
                row.OddStartHouseNumber,
                row.OddEndHouseNumber,
                row.SortOrder,
                row.Direction,
                row.MaxTrailerSize);

            importedCount++;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StreetSectionImportSummary(importedCount, skippedExistingCount, failures);
    }
}
