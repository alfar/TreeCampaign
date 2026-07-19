using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;

namespace TreeTerritory.Api.Neighborhoods;

internal class DeleteStreetSectionEndpoint
{
    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        NeighborhoodId neighborhoodId,
        StreetSectionId streetSectionId,
        ITreeTerritoryUnitOfWork unitOfWork,
        CancellationToken cancellationToken
    )
    {
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
