using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Streets;

internal class GetTerritoryStreetsEndpoint
{
    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        IStreetQueries streetQueries,
        CancellationToken cancellationToken)
    {
        var streets = await streetQueries.GetByTerritoryAsync(territoryId, cancellationToken);
        return Results.Ok(streets);
    }
}
