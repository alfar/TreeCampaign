using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.InfraStructure;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.InfraStructure;
using TreeTerritory.InfraStructure.Queries;
using TerritoryHouseNumber = TreeTerritory.Domain.StreetSections.ValueObjects.HouseNumber;

namespace Intake.InfraStructure.Services;

public class AddressValidationService(
    TreeCampaignContext campaignContext,
    TreeTerritoryContext territoryContext,
    INeighborhoodQueries neighborhoodQueries) : IAddressValidationService
{
    public async Task<AddressValidationResult> ValidateAsync(
        ParsedAddress address, CampaignRef campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await campaignContext.CollectionCampaigns
            .FirstOrDefaultAsync(c => c.Id == CampaignId.From(campaignId.Value), cancellationToken);

        if (campaign?.TerritoryId is null)
            return new StreetNotFound();

        var territoryId = TerritoryId.From(campaign.TerritoryId.Value);

        if (!TerritoryHouseNumber.TryParse(address.HouseNumber, out var houseNumber))
            return new StreetNotFound();

        var streetQuery = territoryContext.Streets
            .Where(s => s.Name.ToLower() == address.Street.ToLower());

        if (address.ZipCode is not null && ZipCode.TryParse(address.ZipCode, out var zipCode))
            streetQuery = streetQuery.Where(s => s.ZipCode == zipCode);

        var streets = await streetQuery.ToListAsync(cancellationToken);

        if (streets.Count == 0)
            return new StreetNotFound();

        var neighborhoods = await neighborhoodQueries.GetAllByTerritoryIdAsync(territoryId, cancellationToken);

        foreach (var street in streets)
        {
            foreach (var neighborhood in neighborhoods)
            {
                var section = neighborhood.StreetSections.FirstOrDefault(s =>
                    s.StreetId == street.Id &&
                    s.StartHouseNumber.CompareTo(houseNumber) <= 0 &&
                    s.EndHouseNumber.CompareTo(houseNumber) >= 0);

                if (section is not null)
                    return new ValidationSuccess(
                        TerritoryRef.From(territoryId.Value),
                        NeighborhoodRef.From(neighborhood.Id.Value),
                        StreetRef.From(street.Id.Value),
                        StreetSectionRef.From(section.Id.Value));
            }
        }

        return new HouseNumberOutOfBounds(StreetRef.From(streets[0].Id.Value));
    }
}
