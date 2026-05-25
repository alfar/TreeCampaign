using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams;

public interface ITeamQueries
{
    Task<IReadOnlyCollection<ProjectionContext.TeamProjection>> GetTeamsAsync(
        CampaignId campaignId,
        CancellationToken cancellationToken
    );
}
