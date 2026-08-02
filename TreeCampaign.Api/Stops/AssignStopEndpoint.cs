using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Stops;

public class AssignStopEndpoint
{
    public record AssignStopCommand(TeamId TeamId);

    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        ICampaignQueries campaignQueries,
        ICurrentUserAccessor currentUser,
        CampaignId campaignId,
        StopId stopId,
        AssignStopCommand command,
        CancellationToken cancellationToken
    )
    {
        if (!await campaignQueries.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var stop = await unitOfWork.GetRepository<UnassignedStop, StopId>().TryFindAsync(stopId, cancellationToken);

        if (stop == null || stop.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        var assignedStop = stop.AssignToTeam(command.TeamId);

        unitOfWork.GetRepository<UnassignedStop, StopId>().Delete(stop);
        unitOfWork.GetRepository<AssignedStop, StopId>().Add(assignedStop);

        var team = await unitOfWork.GetRepository<TeamBase, TeamId>().TryFindAsync(command.TeamId, cancellationToken);
        if (team?.Status == TeamStatus.OnBreak)
        {
            team.ResumeFromBreak();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(assignedStop));
    }
}
