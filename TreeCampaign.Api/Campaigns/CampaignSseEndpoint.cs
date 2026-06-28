using Common.Domain.Abstractions;
using Common.Infrastructure.Services;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Campaigns;

public static class CampaignSseEndpoint
{
    public static IEndpointRouteBuilder MapCampaignSseEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/events", Handle).WithTags("Campaigns");
        return app;
    }

    private static IResult Handle(
        CampaignId campaignId,
        ISseService sseService,
        CancellationToken cancellationToken)
    {
        var stream = sseService.ConnectAsync(
            @event => @event is ICampaignScoped scoped && scoped.CampaignId == campaignId.Value,
            cancellationToken);

        return Results.ServerSentEvents(stream, eventType: "campaign-update");
    }
}
