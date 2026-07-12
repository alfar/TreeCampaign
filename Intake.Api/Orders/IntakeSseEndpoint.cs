using Common.Infrastructure.Services;
using Intake.Domain.Abstractions;
using Intake.Domain.ExternalReferences;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class IntakeSseEndpoint
{
    public static IEndpointRouteBuilder MapIntakeSseEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/events", Handle).WithTags("Orders");
        return app;
    }

    private static IResult Handle(
        [FromRoute] CampaignRef campaignId,
        ISseService sseService,
        CancellationToken cancellationToken)
    {
        var stream = sseService.ConnectAsync(
            @event => @event is IIntakeEvent iie && iie.CampaignId == campaignId,
            cancellationToken);

        return Results.ServerSentEvents(stream, eventType: "intake-update");
    }
}
