using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure;
using static ProjectionContext;

internal class CreateTeamEndpoint
{
    public record CreateTeamCommand(TeamName Name, TeamKind Kind, TrailerSize? TrailerSize);

    internal static async Task<IResult> Handle(
        ICampaignQueries campaignQueries,
        ITreeCampaignUnitOfWork unitOfWork,
        ICurrentUserAccessor currentUser,
        CampaignId campaignId,
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        if (!await campaignQueries.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
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
