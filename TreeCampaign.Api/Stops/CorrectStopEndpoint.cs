using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Repository;

namespace TreeCampaign.Api.Stops;

public class CorrectStopEndpoint
{
    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId,
        CancellationToken cancellationToken
    )
    {
        var stop = await unitOfWork.GetRepository<CollectedStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var correctedStop = stop.CorrectMistakenCollection();

        unitOfWork.GetRepository<CollectedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AssignedStop, StopId>().Add(correctedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(correctedStop));
    }
}
