using System;
using Common.Infrastructure.Auth;

namespace TreeCampaign.Api.Stops;

public static class StopExtensions
{
    public static IEndpointRouteBuilder MapStopEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/stops").WithTags("Stops");
        group.MapGet("/", GetStopsEndpoint.Handle);
        group.MapPost("/", CreateStopEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);
        group.MapPost("/{stopId}/assign", AssignStopEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);
        group.MapPost("/{stopId}/unassign", UnassignStopEndpoint.Handle).RequireAuthorization(AuthPolicies.ScoutGroupMember);
        group.MapPost("/{stopId}/collect", CollectStopEndpoint.Handle);
        group.MapPost("/{stopId}/unresolved", MarkStopUnresolvedEndpoint.Handle);
        group.MapPost("/{stopId}/reopen", ReopenStopEndpoint.Handle);
        group.MapPost("/{stopId}/correct", CorrectStopEndpoint.Handle);
        group.MapPost("/{stopId}/retry", RetryStopEndpoint.Handle);
        group.MapPost("/pickup-request", RequestPickupEndpoint.Handle);

        return app;
    }
}
