using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;

internal class ReportTrailerFullEndpoint
{
    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        TeamId teamId,
        CancellationToken cancellationToken
    )
    {
        var team = await unitOfWork.GetRepository<Team, TeamId>().TryFindAsync(teamId, cancellationToken);
        if (team == null || team.CampaignId != campaignId)
            return TypedResults.NotFound();

        team.ReportTrailerFull();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(team);
    }
}
