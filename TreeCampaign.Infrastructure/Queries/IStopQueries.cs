using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Infrastructure.Queries;

public interface IStopQueries
{
    Task<IReadOnlyCollection<ProjectionContext.StopProjection>> GetStopsAsync(
        CampaignId campaignId
    );
    /// <summary>Total tree count per stop type (see <see cref="State"/>) for the campaign.</summary>
    Task<IReadOnlyDictionary<string, int>> GetTreeCountsByStopTypeAsync(
        CampaignId campaignId,
        CancellationToken cancellationToken
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
        Delivered,
        Abandoned,
    }
}
