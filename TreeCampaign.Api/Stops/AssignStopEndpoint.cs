using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Stops;

public class AssignStopEndpoint
{
    public record AssignStopCommand(TeamId TeamId);

    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        StopId stopId,
        AssignStopCommand command,
        CancellationToken cancellationToken
    )
    {
        var stop = await unitOfWork.GetRepository<UnassignedStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var assignedStop = stop.AssignToTeam(command.TeamId);

        unitOfWork.GetRepository<UnassignedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AssignedStop, StopId>().Add(assignedStop);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(assignedStop));
    }
}
