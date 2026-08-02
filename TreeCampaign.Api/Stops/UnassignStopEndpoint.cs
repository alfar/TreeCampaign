using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Stops;

public class UnassignStopEndpoint
{
    public static async Task<IResult> Handle(
        ICampaignQueries campaignQueries,
        ITreeCampaignUnitOfWork unitOfWork,
        ICurrentUserAccessor currentUser,
        CampaignId campaignId,
        StopId stopId,
        CancellationToken cancellationToken
    )
    {
        if (!await campaignQueries.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return TypedResults.NotFound();
        }

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
