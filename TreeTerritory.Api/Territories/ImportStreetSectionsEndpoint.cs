using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Application.Services;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Territories;

internal class ImportStreetSectionsEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        IFormFile file,
        IStreetSectionImportService importService,
        ITerritoryQueries territoryQueries,
        CancellationToken cancellationToken)
    {
        if (!await territoryQueries.IsOwnedByCurrentScoutGroupAsync(territoryId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        using var reader = new StreamReader(file.OpenReadStream());
        var csvContent = await reader.ReadToEndAsync(cancellationToken);

        var summary = await importService.ImportAsync(territoryId, csvContent, cancellationToken);

        return Results.Ok(summary);
    }
}
