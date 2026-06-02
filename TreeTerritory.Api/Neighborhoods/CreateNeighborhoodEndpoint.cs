using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Repository;

namespace TreeTerritory.Api.Neighborhoods;

internal class CreateNeighborhoodEndpoint
{
    public record CreateNeighborhoodRequest(string Name);

    internal static async Task<IResult> Handle(
        TerritoryId territoryId,
        CreateNeighborhoodRequest request,
        ITreeTerritoryUnitOfWork unitOfWork,
        CancellationToken cancellationToken
    )
    {
        var neighborhoodRepository = unitOfWork.GetRepository<Neighborhood, NeighborhoodId>();

        var neighborhood = Neighborhood.Create(territoryId, request.Name);

        neighborhoodRepository.Add(neighborhood);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(neighborhood);
    }
}
