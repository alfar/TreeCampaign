using Common.Infrastructure.Auth;

namespace TreeTerritory.Api.Neighborhoods;

public static class NeighborhoodExtensions
{
    public static IEndpointRouteBuilder MapNeighborhoodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/neighborhoods").WithTags("Neighborhoods").RequireAuthorization(AuthPolicies.ScoutGroupMember);

        group.MapGet("/", GetNeighborhoodsEndpoint.Handle);

        group.MapPost("/", CreateNeighborhoodEndpoint.Handle);
        
        group.MapPost("/{neighborhoodId:guid}/street-sections", AddStreetSectionToNeighborhoodEndpoint.Handle);

        group.MapPut("/{neighborhoodId:guid}/street-sections/{streetSectionId:guid}", UpdateStreetSectionEndpoint.Handle);

        group.MapDelete("/{neighborhoodId:guid}/street-sections/{streetSectionId:guid}", DeleteStreetSectionEndpoint.Handle);

        return app;
    }
}
