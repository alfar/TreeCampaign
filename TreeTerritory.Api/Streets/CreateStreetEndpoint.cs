using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Repository;

namespace TreeTerritory.Api.Streets;

internal class CreateStreetEndpoint
{
    public record CreateStreetRequest(string Name, ZipCode ZipCode);

    internal static async Task<IResult> Handle(
        CreateStreetRequest request,
        ITreeTerritoryUnitOfWork unitOfWork,
        CancellationToken cancellationToken
    )
    {
        var streetRepository = unitOfWork.GetRepository<Street, StreetId>();

        var street = Street.Create(request.Name, request.ZipCode);

        streetRepository.Add(street);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(street);
    }
}
