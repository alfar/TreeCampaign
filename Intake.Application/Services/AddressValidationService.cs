using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Infrastructure;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure;
using TreeTerritory.Infrastructure.Queries;
using HouseNumber = Intake.Domain.Orders.ValueObjects.HouseNumber;
using TreeTerritoryHouseNumber = TreeTerritory.Domain.StreetSections.ValueObjects.HouseNumber;
using StreetSectionRef = Intake.Domain.ExternalReferences.StreetSectionRef;
using TerritoryRef = Intake.Domain.ExternalReferences.TerritoryRef;

namespace Intake.Application.Services;

public class AddressValidationService(
    TreeCampaignContext campaignContext,
    TreeTerritoryContext territoryContext,
    INeighborhoodQueries neighborhoodQueries,
    IAddressLookupClient addressLookupClient) : IAddressValidationService
{
    public async Task<AddressValidationResult> ValidateAsync(
        ParsedAddress address, CampaignRef campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await campaignContext.CollectionCampaigns
            .FirstOrDefaultAsync(c => c.Id == CampaignId.From(campaignId.Value), cancellationToken);

        if (campaign?.TerritoryId is null)
            return new StreetNotFound();

        var territoryId = TerritoryId.From(campaign.TerritoryId.Value);

        if (!TreeTerritoryHouseNumber.TryParse(address.HouseNumber, out var territoryHouseNumber))
            return new StreetNotFound();

        var streetQuery = territoryContext.Streets
            .Where(s => EF.Functions.Like(s.Name, address.Street.ToLower()));

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
                    s.ContainsHouseNumber(territoryHouseNumber));

                if (section is not null)
                {

                    var lookup = await addressLookupClient.GetAddress(address.Street, address.HouseNumber, address.ZipCode ?? "8600");

                    if (lookup is not null)
                    {
                        return new ValidationSuccess(
                            TerritoryRef.From(territoryId.Value),
                            NeighborhoodRef.From(neighborhood.Id.Value),
                            StreetRef.From(street.Id.Value),
                            StreetSectionRef.From(section.Id.Value),
                            ToIntake(territoryHouseNumber),
                            lookup.Latitude, 
                            lookup.Longitude
                        );
                    }

                }
            }
        }

        return new HouseNumberOutOfBounds(StreetRef.From(streets[0].Id.Value), HouseNumber.Parse(address.HouseNumber));
    }

    public async Task<AddressValidationResult> ValidateRefsAsync(
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber,
        CampaignRef campaignId,
        CancellationToken cancellationToken = default)
    {
        var campaign = await campaignContext.GetRepository<Campaign, CampaignId>()
           .TryFindAsync(CampaignId.From(campaignId.Value), cancellationToken);

        if (campaign?.TerritoryId is null)
            return new StreetNotFound();

        var territoryId = TerritoryId.From(campaign.TerritoryId.Value);

        var street = await territoryContext.GetRepository<Street, StreetId>()
            .TryFindAsync(StreetId.From(streetId.Value), cancellationToken);

        if (street is null)
            return new StreetNotFound();

        var neighborhood = await territoryContext.GetRepository<Neighborhood, NeighborhoodId>()
            .TryFindAsync(NeighborhoodId.From(neighborhoodId.Value), cancellationToken);

        if (neighborhood is null)
            return new StreetNotFound();

        var section = neighborhood.StreetSections.FirstOrDefault(s => s.Id == StreetSectionId.From(streetSectionId.Value) && s.StreetId == street.Id);

        if (section is null)
            return new StreetNotFound();

        var territoryHouseNumber = ToTerritory(houseNumber);
        if (!section.ContainsHouseNumber(territoryHouseNumber))
        {
            return new HouseNumberOutOfBounds(streetId, houseNumber);
        }

        var lookup = await addressLookupClient.GetAddress(street.Name, houseNumber.ToString(), street.ZipCode.Value);

        if (lookup is null)
        {
            return new HouseNumberOutOfBounds(streetId, houseNumber);
        }

        return new ValidationSuccess(
            TerritoryRef.From(territoryId.Value),
            NeighborhoodRef.From(neighborhood.Id.Value),
            StreetRef.From(street.Id.Value),
            StreetSectionRef.From(section.Id.Value),
            houseNumber,
            lookup.Latitude,
            lookup.Longitude
        );
    }

    private static HouseNumber ToIntake(TreeTerritoryHouseNumber input) =>
        HouseNumber.Parse(input.ToString());
    
    private static TreeTerritoryHouseNumber ToTerritory(HouseNumber input) =>
        TreeTerritoryHouseNumber.Parse(input.ToString());
}
