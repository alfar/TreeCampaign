using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using Common.Repository.Abstractions;

namespace TreeCampaign.Api.Stops;

public class MarkStopUnresolvedEndpoint
{
    public record MarkStopUnresolvedCommand(ReasonText Reason);

    public static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId,
        MarkStopUnresolvedCommand command,
        CancellationToken cancellationToken
    )
    {
        var stop = await unitOfWork.GetRepository<AssignedStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var unresolvedStop = stop.MarkUnresolved(command.Reason);

        unitOfWork.GetRepository<AssignedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<UnresolvedStop, StopId>().Add(unresolvedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(unresolvedStop));
    }
}
