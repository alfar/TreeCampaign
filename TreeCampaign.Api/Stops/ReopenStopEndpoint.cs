using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.InfraStructure;

namespace TreeCampaign.Api.Stops;

public class ReopenStopEndpoint
{
    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId,
        CancellationToken cancellationToken
    )
    {
        var stop = await unitOfWork.GetRepository<ReopenableStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var unassignedStop = stop.Reopen();

        unitOfWork.GetRepository<ReopenableStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<UnassignedStop, StopId>().Add(unassignedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(unassignedStop));
    }
}
