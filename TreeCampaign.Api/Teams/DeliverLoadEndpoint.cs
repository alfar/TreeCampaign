using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;

internal class DeliverLoadEndpoint
{
    internal static async Task<IResult> Handle(
        TreeCampaignContext context,
        CampaignId campaignId,
        TeamId teamId,
        CancellationToken cancellationToken
    )
    {
        var team = await context.GetRepository<TrailerTeam, TeamId>().TryFindAsync(teamId, cancellationToken);
        if (team == null || team.CampaignId != campaignId)
            return TypedResults.NotFound();

        var collectedStops = await context.CollectedStops
            .Where(s => s.CollectedByTeamId == teamId && s.CampaignId == campaignId)
            .ToListAsync(cancellationToken);

        if (collectedStops.Count == 0)
            return TypedResults.Ok(new { deliveredCount = 0 });

        var deliveredStops = collectedStops.Select(s => s.Deliver()).ToList();

        foreach (var (collected, delivered) in collectedStops.Zip(deliveredStops))
        {
            context.GetRepository<CollectedStop, StopId>().Delete(collected);
            context.GetRepository<DeliveredStop, StopId>().Add(delivered);
        }

        team.ClearTrailerFull();

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(new { deliveredCount = deliveredStops.Count });
    }
}
