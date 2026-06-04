public static class TeamExtensions
{
    public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/teams").WithTags("Teams");

        group.MapGet("/", GetTeamsEndpoint.Handle);

        group.MapPost("/", CreateTeamEndpoint.Handle);
        group.MapPut("/{teamId:guid}", UpdateTeamEndpoint.Handle);

        return app;
    }
}
