using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.StreetSections;

internal class ResolveStreetSectionEndpoint
{
    internal record StreetSectionMatch(
        Guid TerritoryId,
        Guid StreetId,
        Guid StreetSectionId,
        Guid NeighborhoodId,
        string HouseNumber);

    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        StreetId streetId,
        string houseNumber,
        ITerritoryQueries territoryQueries,
        IStreetSectionQueries streetSectionQueries,
        CancellationToken cancellationToken)
    {
        var territory = await territoryQueries.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return Results.NotFound();
        }

        if (!HouseNumber.TryParse(houseNumber, out var parsedHouseNumber))
        {
            return Results.BadRequest("Invalid house number.");
        }

        var sections = await streetSectionQueries.GetByTerritoryAndStreetAsync(territoryId, streetId, cancellationToken);
        var match = sections.FirstOrDefault(section => section.ContainsHouseNumber(parsedHouseNumber));

        if (match is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new StreetSectionMatch(
            territoryId.Value,
            streetId.Value,
            match.Id.Value,
            match.NeighborhoodId.Value,
            parsedHouseNumber.ToString()));
    }
}
