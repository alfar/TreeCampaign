using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Neighborhoods;

internal class DeleteStreetSectionEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        NeighborhoodId neighborhoodId,
        StreetSectionId streetSectionId,
        ITreeTerritoryUnitOfWork unitOfWork,
        ITerritoryQueries territoryQueries,
        CancellationToken cancellationToken
    )
    {
        if (!await territoryQueries.IsOwnedByCurrentScoutGroupAsync(territoryId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var neighborhoodRepository = unitOfWork.GetRepository<Neighborhood, NeighborhoodId>();

        var neighborhood = await neighborhoodRepository.TryFindAsync(neighborhoodId, cancellationToken);

        if (neighborhood is null)
        {
            return Results.NotFound();
        }

        if (!neighborhood.StreetSections.Any(s => s.Id == streetSectionId))
        {
            return Results.NotFound();
        }

        neighborhood.RemoveStreetSection(streetSectionId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(neighborhood);
    }
}
