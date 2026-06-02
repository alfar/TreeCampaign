using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.InfraStructure;

internal class CreateTeamEndpoint
{
    public record CreateTeamCommand(TeamName Name);

    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        var team = Team.Create(campaignId, command.Name);

        unitOfWork.GetRepository<Team, TeamId>().Add(team);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(team);
    }
}
