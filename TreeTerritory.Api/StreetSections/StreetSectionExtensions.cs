using Common.Infrastructure.Auth;
using TreeTerritory.Api.Streets;

namespace TreeTerritory.Api.StreetSections;

public static class StreetSectionExtensions
{
    public static IEndpointRouteBuilder MapStreetSectionEndpoints(this IEndpointRouteBuilder app)
    {
        var territoryGroup = app.MapGroup("/territories/{territoryId:guid}");

        territoryGroup.MapGroup("/street-sections")
            .WithTags("StreetSections")
            .MapGet("/resolve", ResolveStreetSectionEndpoint.Handle);

        territoryGroup.MapGet("/streets", GetTerritoryStreetsEndpoint.Handle)
            .WithTags("Streets");

        var group = app.MapGroup("/streets/{streetId:guid}")
            .WithTags("StreetSections")
            .RequireAuthorization(AuthPolicies.ScoutGroupMember);

        group.MapGet("/sections", GetStreetSectionsEndpoint.Handle);

        return app;
    }
}
