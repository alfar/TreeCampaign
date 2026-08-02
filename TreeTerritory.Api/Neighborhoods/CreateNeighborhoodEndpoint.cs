using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Neighborhoods;

internal class CreateNeighborhoodEndpoint
{
    public record CreateNeighborhoodRequest(string Name);

    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        CreateNeighborhoodRequest request,
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

        var neighborhood = Neighborhood.Create(territoryId, request.Name);

        neighborhoodRepository.Add(neighborhood);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(neighborhood);
    }
}
