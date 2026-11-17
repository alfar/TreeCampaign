using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure.Queries;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Infrastructure.Queries;

namespace TreeCampaign.Application;

/// <summary>
/// Returns a team's stops in route order. Section order/direction lives in Territory, so this
/// application service is the seam that joins the two contexts (read-model only).
/// </summary>
public class TeamStopsService(IStopQueries stopQueries, IStreetSectionQueries streetSectionQueries)
{
    public async Task<IReadOnlyCollection<ProjectionContext.StopProjection>> GetSortedStopsForTeamAsync(
        CampaignId campaignId,
        TeamId teamId,
        CancellationToken cancellationToken)
    {
        var stops = await stopQueries.GetStopsByTeamIdAsync(campaignId, teamId);

        var sectionIds = stops
            .Select(s => s.Address.StreetSectionId.Value)
            .Distinct()
            .Select(StreetSectionId.From)
            .ToList();

        var sections = (await streetSectionQueries.GetByIdsAsync(sectionIds, cancellationToken))
            .ToDictionary(
                s => s.Id.Value,
                s => new SectionOrder(s.SortOrder, s.Direction == Direction.Descending));

        var comparer = StopOrdering.Comparer(sections);
        return [.. stops.OrderBy(s => s.Address, comparer)];
    }
}
