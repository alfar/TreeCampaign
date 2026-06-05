using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Territories;

internal class GetTerritoriesEndpoint
{
    internal static async Task<IResult> Handle(
        ITerritoryQueries territoryQueries,
        CancellationToken cancellationToken
    )
    {
        var territories = await territoryQueries.GetAllAsync(cancellationToken);
        return Results.Ok(territories);
    }
}
