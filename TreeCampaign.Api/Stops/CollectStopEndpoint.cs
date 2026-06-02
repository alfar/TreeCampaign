using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.InfraStructure;

namespace TreeCampaign.Api.Stops;

public class CollectStopEndpoint
{
    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId,
        CancellationToken cancellationToken
    )
    {
        var stop = await unitOfWork.GetRepository<AssignedStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var collectedStop = stop.Collect();

        unitOfWork.GetRepository<AssignedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<CollectedStop, StopId>().Add(collectedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(collectedStop));
    }
}
