using Common.Infrastructure.Auth;

namespace TreeTerritory.Api.Territories;

public static class TerritoryExtensions
{
    public static IEndpointRouteBuilder MapTerritoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/territories").WithTags("Territories").RequireAuthorization(AuthPolicies.ScoutGroupMember);

        group.MapGet("/", GetTerritoriesEndpoint.Handle);
        group.MapGet("/{territoryId:guid}", GetTerritoryEndpoint.Handle);

        group.MapPost("/", CreateTerritoryEndpoint.Handle);

        // Unauthenticated: the team detail route needs this to prefill the pickup form's zip code for scouts in the field who aren't logged in.
        app.MapGroup("/territories")
            .WithTags("Territories")
            .MapGet("/{territoryId:guid}/summary", GetTerritorySummaryEndpoint.Handle);

        return app;
    }
}
