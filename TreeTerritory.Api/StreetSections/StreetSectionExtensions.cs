using Common.Infrastructure.Auth;

namespace TreeTerritory.Api.StreetSections;

public static class StreetSectionExtensions
{
    public static IEndpointRouteBuilder MapStreetSectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/streets/{streetId:guid}").WithTags("StreetSections").RequireAuthorization(AuthPolicies.ScoutGroupMember);

        group.MapGet("/sections", GetStreetSectionsEndpoint.Handle);

        return app;
    }
}
