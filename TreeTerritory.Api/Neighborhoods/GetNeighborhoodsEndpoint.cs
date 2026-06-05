using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Neighborhoods;

internal class GetNeighborhoodsEndpoint
{
    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        INeighborhoodQueries neighborhoodQueries,
        CancellationToken cancellationToken
    )
    {
        var neighborhoods = await neighborhoodQueries.GetAllByTerritoryIdAsync(territoryId, cancellationToken);
        return Results.Ok(neighborhoods);
    }
}
