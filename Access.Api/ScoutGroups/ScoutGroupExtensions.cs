namespace Access.Api.ScoutGroups;

public static class ScoutGroupExtensions
{
    public static IEndpointRouteBuilder MapScoutGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/scoutgroups").WithTags("ScoutGroups");

        group.MapPost("/", CreateScoutGroupEndpoint.Handle);

        return app;
    }
}
