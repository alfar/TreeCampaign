using System;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams;
using Common.Repository.Abstractions;

namespace TreeCampaign.Api.Stops;

public class UnassignStopEndpoint
{
    public static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
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

        var unassignedStop = stop.Unassign();

        unitOfWork.GetRepository<AssignedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<UnassignedStop, StopId>().Add(unassignedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(unassignedStop));
    }
}
