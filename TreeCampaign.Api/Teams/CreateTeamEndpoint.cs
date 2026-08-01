using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;
using static ProjectionContext;

internal class CreateTeamEndpoint
{
    public record CreateTeamCommand(TeamName Name, TeamKind Kind, TrailerSize? TrailerSize);

    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        var campaign = await unitOfWork.GetRepository<Campaign, CampaignId>().TryFindAsync(campaignId, cancellationToken);

        if (campaign is null || campaign.ScoutGroupId != currentUser.GetScoutGroupId())
        {
            return Results.NotFound();
        }

        if (command.Kind == TeamKind.Trailer && command.TrailerSize is null)
        {
            return TypedResults.BadRequest("TrailerSize is required for trailer teams.");
        }

        TeamBase team = command.Kind switch
        {
            TeamKind.Walking => WalkingTeam.Create(campaignId, command.Name),
            TeamKind.Trailer => TrailerTeam.Create(campaignId, command.Name, command.TrailerSize!.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(command.Kind)),
        };

        unitOfWork.GetRepository<TeamBase, TeamId>().Add(team);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(TeamProjection.FromTeam(team));
    }
}
