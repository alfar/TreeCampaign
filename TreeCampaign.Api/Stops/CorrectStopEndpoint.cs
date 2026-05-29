using System;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams;
using Common.Repository.Abstractions;

namespace TreeCampaign.Api.Stops;

public class CorrectStopEndpoint
{
    public static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId
    )
    {
        var stop = await unitOfWork.GetRepository<CollectedStop, StopId>().TryFindAsync(stopId);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var correctedStop = stop.CorrectMistakenCollection();

        unitOfWork.GetRepository<CollectedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AssignedStop, StopId>().Add(correctedStop);

        await unitOfWork.SaveChangesAsync();

        return TypedResults.Ok(ProjectionContext.StopProjection.From(correctedStop));
    }
}
