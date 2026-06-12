using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Territories;

internal class GetTerritoryEndpoint
{
    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        ITerritoryQueries territoryQueries,
        CancellationToken cancellationToken
    )
    {
        var territory = await territoryQueries.GetByIdAsync(territoryId, cancellationToken);
        return territory is null ? Results.NotFound() : Results.Ok(territory);
    }
}
