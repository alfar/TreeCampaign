using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Repository;
using TreeCampaign.Repository.Abstractions;

internal class CreateTeamEndpoint
{
    public record CreateTeamCommand(TeamName Name);

    internal static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CampaignId campaignId,
        CreateTeamCommand command
    )
    {
        var team = Team.Create(campaignId, command.Name);

        unitOfWork.GetRepository<Team, TeamId>().Add(team);
        await unitOfWork.SaveChangesAsync();

        return TypedResults.Ok(team);
    }
}
