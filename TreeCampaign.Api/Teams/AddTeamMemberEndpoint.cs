using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;

internal class AddTeamMemberEndpoint
{
    public record AddTeamMemberCommand(string Name, string? ScoutRelativeName, string PhoneNumber);

    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        TeamId teamId,
        AddTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var team = await unitOfWork.GetRepository<Team, TeamId>().TryFindAsync(teamId, cancellationToken);
        if (team == null || team.CampaignId != campaignId)
            return TypedResults.NotFound();

        team.AddMember(command.Name, command.ScoutRelativeName, command.PhoneNumber);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(team);
    }
}
