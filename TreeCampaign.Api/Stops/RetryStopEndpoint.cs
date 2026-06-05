using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Stops;

public class RetryStopEndpoint
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

        var retriedStop = stop.Retry();

        unitOfWork.GetRepository<UnresolvedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AssignedStop, StopId>().Add(retriedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(retriedStop));
    }
}
