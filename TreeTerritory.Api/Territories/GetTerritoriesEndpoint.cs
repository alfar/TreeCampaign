using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Territories;

internal class GetTerritoriesEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        ITerritoryQueries territoryQueries,
        CancellationToken cancellationToken
    )
    {
        var territories = await territoryQueries.GetAllAsync(currentUser.GetScoutGroupId(), cancellationToken);
        return Results.Ok(territories);
    }
}
