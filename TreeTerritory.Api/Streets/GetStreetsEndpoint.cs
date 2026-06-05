using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeTerritory.Api.Streets;

internal class GetStreetsEndpoint
{
    internal static async Task<IResult> Handle(
        ZipCode zipCode,
        IStreetQueries streetQueries,
        CancellationToken cancellationToken
    )
    {
        var streets = await streetQueries.GetByZipCodeAsync(zipCode, cancellationToken);
        return Results.Ok(streets);
    }
}
