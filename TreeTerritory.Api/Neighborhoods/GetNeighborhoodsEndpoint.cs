using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Neighborhoods;

internal class GetNeighborhoodsEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        ITerritoryQueries territoryQueries,
        INeighborhoodQueries neighborhoodQueries,
        CancellationToken cancellationToken
    )
    {
        if (!await territoryQueries.IsOwnedByCurrentScoutGroupAsync(territoryId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }
        
        var neighborhoods = await neighborhoodQueries.GetAllByTerritoryIdAsync(territoryId, cancellationToken);
        return Results.Ok(neighborhoods);
    }
}
