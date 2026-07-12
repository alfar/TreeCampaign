using Common.Infrastructure.Services;
using TreeCampaign.Domain.Abstractions;
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
            @event => @event is ITreeCampaignEvent tce && tce.CampaignId == campaignId,
            cancellationToken);

        return Results.ServerSentEvents(stream, eventType: "campaign-update");
    }
}
