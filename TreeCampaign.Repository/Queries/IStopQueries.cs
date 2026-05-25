using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Repository.Queries;

public interface IStopQueries
{
    Task<IReadOnlyCollection<ProjectionContext.StopProjection>> GetStopsAsync(
        CampaignId campaignId
    );
    Task<IReadOnlyCollection<ProjectionContext.StopProjection>> GetStopsByTeamIdAsync(
        CampaignId campaignId,
        TeamId teamId
    );

    public enum State
    {
        Unassigned,
        Assigned,
        Collected,
        Unresolved,
    }
}
