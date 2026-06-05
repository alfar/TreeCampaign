using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;

namespace TreeTerritory.Api.Territories;

internal class CreateTerritoryEndpoint
{
    public record CreateTerritoryRequest(string Name);

    internal static async Task<IResult> Handle(
        CreateTerritoryRequest request,
        ITreeTerritoryUnitOfWork unitOfWork,
        CancellationToken cancellationToken
    )
    {
        var territoryRepository = unitOfWork.GetRepository<Territory, TerritoryId>();

        var territory = Territory.Create(request.Name);

        territoryRepository.Add(territory);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(territory);
    }
}
