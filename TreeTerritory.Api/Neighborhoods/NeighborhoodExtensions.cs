namespace TreeTerritory.Api.Neighborhoods;

public static class NeighborhoodExtensions
{
    public static IEndpointRouteBuilder MapNeighborhoodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/neighborhoods").WithTags("Neighborhoods");

        group.MapGet("/", GetNeighborhoodsEndpoint.Handle);

        group.MapPost("/", CreateNeighborhoodEndpoint.Handle);
        
        group.MapPost("/{neighborhoodId:guid}/street-sections", AddStreetSectionToNeighborhoodEndpoint.Handle);

        return app;
    }
}
