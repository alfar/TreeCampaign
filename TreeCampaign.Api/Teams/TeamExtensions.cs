using Common.Infrastructure.Auth;

public static class TeamExtensions
{
    public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/teams").WithTags("Teams");

        group.MapGet("/", GetTeamsEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);
        group.MapPost("/", CreateTeamEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);
        group.MapPost("/{teamId:guid}/break", GoOnBreakEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);
        group.MapDelete("/{teamId:guid}/break", ReturnFromBreakEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);

        group.MapGet("/{teamId:guid}", GetTeamEndpoint.Handle);
        group.MapPut("/{teamId:guid}", UpdateTeamEndpoint.Handle);

        group.MapPost("/{teamId:guid}/members", AddTeamMemberEndpoint.Handle);
        group.MapDelete("/{teamId:guid}/members/{memberId:guid}", RemoveTeamMemberEndpoint.Handle);

        group.MapPost("/{teamId:guid}/trailer-full", ReportTrailerFullEndpoint.Handle);
        group.MapDelete("/{teamId:guid}/trailer-full", ClearTrailerFullEndpoint.Handle);
        group.MapPost("/{teamId:guid}/deliver-load", DeliverLoadEndpoint.Handle);
        group.MapPost("/{teamId:guid}/extra-trees", AdjustExtraTreesEndpoint.Handle);
        group.MapPost("/{teamId:guid}/pickup-request", RequestPickupEndpoint.Handle);

        return app;
    }
}
