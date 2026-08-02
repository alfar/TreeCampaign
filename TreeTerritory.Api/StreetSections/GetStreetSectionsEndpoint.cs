using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.StreetSections;

internal class GetStreetSectionsEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        StreetId streetId,
        ITerritoryQueries territoryQueries,
        IStreetSectionQueries streetSectionQueries,
        CancellationToken cancellationToken
    )
    {
        if (!await territoryQueries.IsOwnedByCurrentScoutGroupAsync(territoryId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var sections = await streetSectionQueries.GetByTerritoryAndStreetAsync(territoryId, streetId, cancellationToken);
        return Results.Ok(sections);
    }
}
