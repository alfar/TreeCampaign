using Common.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Infrastructure;
using TreeCampaign.Infrastructure.Queries;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using TreeTerritory.Infrastructure.Queries;
using TerritoryHouseNumber = TreeTerritory.Domain.StreetSections.ValueObjects.HouseNumber;

namespace TreeCampaign.Application;

public class PickupRequestService(
    ICampaignQueries campaignQueries,
    TreeTerritoryContext territoryContext,
    IStreetSectionQueries streetSectionQueries,
    IAddressLookupClient addressLookupClient,
    ITreeCampaignUnitOfWork unitOfWork)
{
    public async Task<ProjectionContext.StopProjection?> RequestPickupAsync(
        CampaignId campaignId,
        Guid streetId,
        string houseNumber,
        int treeCount,
        CancellationToken cancellationToken)
    {
        var campaign = await campaignQueries.GetByIdAsync(campaignId, cancellationToken);
        if (campaign?.TerritoryId is null)
            return null;

        var street = await territoryContext.Streets
            .FirstOrDefaultAsync(s => s.Id == StreetId.From(streetId), cancellationToken);
        if (street is null)
            return null;

        if (!TerritoryHouseNumber.TryParse(houseNumber, out var territoryHouseNumber))
            return null;

        var sections = await streetSectionQueries.GetByTerritoryAndStreetAsync(
            TerritoryId.From(campaign.TerritoryId.Value), street.Id, cancellationToken);

        var matchingSection = sections.FirstOrDefault(s => s.ContainsHouseNumber(territoryHouseNumber));
        if (matchingSection is null)
            return null;

        var lookup = await addressLookupClient.GetAddress(street.Name, houseNumber, street.ZipCode.Value);
        if (lookup is null)
            return null;

        var address = new Address(
            $"{lookup.Street} {lookup.HouseNumber}, {lookup.ZipCode}",
            lookup.Latitude,
            lookup.Longitude,
            StreetSectionRef.From(matchingSection.Id.Value));

        var stop = UnassignedStop.Create(campaignId, address, TreeCount.From(treeCount));
        unitOfWork.GetRepository<UnassignedStop, StopId>().Add(stop);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProjectionContext.StopProjection.From(stop);
    }
}
