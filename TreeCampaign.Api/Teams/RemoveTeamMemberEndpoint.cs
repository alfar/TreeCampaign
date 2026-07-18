using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.TeamMembers.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;
using static ProjectionContext;

internal class RemoveTeamMemberEndpoint
{
    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        TeamId teamId,
        TeamMemberId memberId,
        CancellationToken cancellationToken
    )
    {
        var team = await unitOfWork.GetRepository<TeamBase, TeamId>().TryFindAsync(teamId, cancellationToken);
        if (team == null || team.CampaignId != campaignId)
            return TypedResults.NotFound();

        team.RemoveMember(memberId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(TeamProjection.FromTeam(team));
    }
}
