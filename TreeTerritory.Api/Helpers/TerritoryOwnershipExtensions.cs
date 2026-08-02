using Common.Infrastructure.Auth;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Helpers;

internal static class TerritoryOwnershipExtensions
{
    internal static async Task<bool> IsOwnedByCurrentScoutGroupAsync(
        this ITerritoryQueries territoryQueries,
        TerritoryId territoryId,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken
    )
    {
        var territory = await territoryQueries.GetByIdAsync(territoryId, cancellationToken);

        return territory is not null && territory.ScoutGroupId == currentUser.GetScoutGroupId();
    }
}
