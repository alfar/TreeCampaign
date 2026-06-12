namespace TreeTerritory.Api.Territories;

public static class TerritoryExtensions
{
    public static IEndpointRouteBuilder MapTerritoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/territories").WithTags("Territories");

        group.MapGet("/", GetTerritoriesEndpoint.Handle);
        group.MapGet("/{territoryId:guid}", GetTerritoryEndpoint.Handle);

        group.MapPost("/", CreateTerritoryEndpoint.Handle);

        return app;
    }
}
