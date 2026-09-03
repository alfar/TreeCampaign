using Common.Infrastructure.Auth;
using Common.Infrastructure.Services;
using Intake.Api.Helpers;
using Intake.Domain.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class IntakeSseEndpoint
{
    public static IEndpointRouteBuilder MapIntakeSseEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/events", Handle).WithTags("Orders");
        return app;
    }

    private static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        ISseService sseService,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        if (!await campaignOwnershipService.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var stream = sseService.ConnectAsync(
            @event => @event is IIntakeEvent iie && iie.CampaignId == campaignId,
            cancellationToken);

        return Results.ServerSentEvents(stream, eventType: "intake-update");
    }
}
