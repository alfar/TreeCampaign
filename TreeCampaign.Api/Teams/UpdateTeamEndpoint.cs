using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Repository.Abstractions;

internal class UpdateTeamEndpoint
{
    public record UpdateTeamCommand(TeamName Name);

    internal static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CampaignId campaignId,
        TeamId teamId,
        UpdateTeamCommand command
    )
    {
        var team = await unitOfWork.GetRepository<Team, TeamId>().TryFindAsync(teamId);
        if (team == null || team.CampaignId != campaignId)
        {
            return TypedResults.NotFound();
        }

        team.UpdateName(command.Name);
        await unitOfWork.SaveChangesAsync();

        return TypedResults.Ok(team);
    }
}
