using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;

public interface ITeamQueries
{
    Task<IReadOnlyCollection<ProjectionContext.TeamProjection>> GetTeamsAsync(
        CampaignId campaignId,
        CancellationToken cancellationToken
    );

    Task<ProjectionContext.TeamProjection?> GetTeamAsync(
        TeamId teamId,
        CancellationToken cancellationToken
    );
}
