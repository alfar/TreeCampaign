public static class TeamExtensions
{
    public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/teams").WithTags("Teams");

        group.MapGet("/", GetTeamsEndpoint.Handle);

        group.MapPost("/", CreateTeamEndpoint.Handle);
        group.MapPut("/{teamId:guid}", UpdateTeamEndpoint.Handle);

        group.MapPost("/{teamId:guid}/members", AddTeamMemberEndpoint.Handle);
        group.MapDelete("/{teamId:guid}/members/{memberId:guid}", RemoveTeamMemberEndpoint.Handle);

        group.MapPost("/{teamId:guid}/break", GoOnBreakEndpoint.Handle);
        group.MapPost("/{teamId:guid}/trailer-full", ReportTrailerFullEndpoint.Handle);
        group.MapPost("/{teamId:guid}/deliver-load", DeliverLoadEndpoint.Handle);

        return app;
    }
}
