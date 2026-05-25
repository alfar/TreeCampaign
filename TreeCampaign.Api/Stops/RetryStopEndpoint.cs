using System;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Repository.Abstractions;

namespace TreeCampaign.Api.Stops;

public class RetryStopEndpoint
{
    public static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId
    )
    {
        var stop = await unitOfWork.GetRepository<UnresolvedStop, StopId>().TryFindAsync(stopId);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var retriedStop = stop.Retry();

        unitOfWork.GetRepository<UnresolvedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AssignedStop, StopId>().Add(retriedStop);

        await unitOfWork.SaveChangesAsync();

        return TypedResults.Ok(ProjectionContext.StopProjection.From(retriedStop));
    }
}
