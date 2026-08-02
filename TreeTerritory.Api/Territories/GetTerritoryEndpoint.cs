using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Territories;

internal class GetTerritoryEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        ITerritoryQueries territoryQueries,
        CancellationToken cancellationToken
    )
    {
        var territory = await territoryQueries.GetByIdAsync(territoryId, cancellationToken);
        return territory is null || territory.ScoutGroupId != currentUser.GetScoutGroupId() ? Results.NotFound() : Results.Ok(territory);
    }
}
