using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using Common.Infrastructure.Auth;
using TreeTerritory.Api.Helpers;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Neighborhoods;

internal class AddStreetSectionToNeighborhoodEndpoint
{
    public record AddStreetSectionToNeighborhoodRequest(
        StreetId StreetId,
        int SortOrder,
        HouseNumber? EvenFromHouseNumber,
        HouseNumber? EvenToHouseNumber,
        HouseNumber? OddFromHouseNumber,
        HouseNumber? OddToHouseNumber,
        Direction Direction,
        TrailerSize MaxTrailerSize = TrailerSize.Boogie);

    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        TerritoryId territoryId,
        NeighborhoodId neighborhoodId,
        AddStreetSectionToNeighborhoodRequest request,
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

        neighborhood.AddStreetSection(
            request.StreetId,
            request.EvenFromHouseNumber,
            request.EvenToHouseNumber,
            request.OddFromHouseNumber,
            request.OddToHouseNumber,
            request.SortOrder,
            request.Direction,
            request.MaxTrailerSize);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(neighborhood);
    }
}
