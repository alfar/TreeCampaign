using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;
using static ProjectionContext;

internal class GoOnBreakEndpoint
{
    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        ICurrentUserAccessor currentUser,
        TeamId teamId,
        CancellationToken cancellationToken
    )
    {
        if (!await unitOfWork.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var team = await unitOfWork.GetRepository<TeamBase, TeamId>().TryFindAsync(teamId, cancellationToken);
        if (team == null || team.CampaignId != campaignId)
            return TypedResults.NotFound();

        team.GoOnBreak();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(TeamProjection.FromTeam(team));
    }
}
