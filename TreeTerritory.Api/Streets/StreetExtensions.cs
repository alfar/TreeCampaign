namespace TreeTerritory.Api.Streets;

public static class StreetExtensions
{
    public static IEndpointRouteBuilder MapStreetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/streets").WithTags("Streets");

        group.MapGet("/{zipCode}", GetStreetsEndpoint.Handle);

        group.MapPost("/", CreateStreetEndpoint.Handle);

        return app;
    }
}
