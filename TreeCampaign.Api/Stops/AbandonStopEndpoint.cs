using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Stops;

public class AbandonStopEndpoint
{
    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId,
        CancellationToken cancellationToken
    )
    {
        var stop = await unitOfWork.GetRepository<UnresolvedStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var abandonedStop = stop.Abandon();

        unitOfWork.GetRepository<UnresolvedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AbandonedStop, StopId>().Add(abandonedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(abandonedStop));
    }
}
