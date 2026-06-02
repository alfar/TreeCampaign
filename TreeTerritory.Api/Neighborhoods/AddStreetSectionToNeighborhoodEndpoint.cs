using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.InfraStructure;

namespace TreeTerritory.Api.Neighborhoods;

internal class AddStreetSectionToNeighborhoodEndpoint
{
    public record AddStreetSectionToNeighborhoodRequest(StreetId StreetId, int SortOrder, HouseNumber FromHouseNumber, HouseNumber ToHouseNumber, Direction Direction);

    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        NeighborhoodId neighborhoodId,
        AddStreetSectionToNeighborhoodRequest request,
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

        neighborhood.AddStreetSection(request.StreetId, request.FromHouseNumber, request.ToHouseNumber, request.SortOrder, request.Direction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(neighborhood);
    }
}
