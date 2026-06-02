using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Repository;

internal class UpdateTeamEndpoint
{
    public record UpdateTeamCommand(TeamName Name);

    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        TeamId teamId,
        UpdateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        var team = await unitOfWork.GetRepository<Team, TeamId>().TryFindAsync(teamId, cancellationToken);
        if (team == null || team.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        team.UpdateName(command.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(team);
    }
}
