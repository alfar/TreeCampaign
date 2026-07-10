using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;

internal class CreateTeamEndpoint
{
    public record CreateTeamCommand(TeamName Name, TeamKind Kind);

    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        TeamBase team = command.Kind switch
        {
            TeamKind.Walking => WalkingTeam.Create(campaignId, command.Name),
            TeamKind.Trailer => TrailerTeam.Create(campaignId, command.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(command.Kind)),
        };

        unitOfWork.GetRepository<TeamBase, TeamId>().Add(team);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(team);
    }
}
