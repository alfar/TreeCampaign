using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.StreetSections;

internal class GetStreetSectionsEndpoint
{
    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        StreetId streetId,
        IStreetSectionQueries streetSectionQueries,
        CancellationToken cancellationToken
    )
    {
        var sections = await streetSectionQueries.GetByTerritoryAndStreetAsync(territoryId, streetId, cancellationToken);
        return Results.Ok(sections);
    }
}
