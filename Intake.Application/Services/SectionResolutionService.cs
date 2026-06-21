

using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Infrastructure.Queries;
using TerritoryHouseNumber = TreeTerritory.Domain.StreetSections.ValueObjects.HouseNumber;
using StreetSectionRef = Intake.Domain.ExternalReferences.StreetSectionRef;

namespace Intake.Application.Services;

public class SectionResolutionService : ISectionResolutionService
{
    private readonly ICampaignQueries _campaignQueries;
    private readonly IStreetSectionQueries _streetSectionQueries;

    public SectionResolutionService(ICampaignQueries campaignQueries, IStreetSectionQueries streetSectionQueries)
    {
        _campaignQueries = campaignQueries;
        _streetSectionQueries = streetSectionQueries;
    }

    public async Task<SectionResolutionResultBase?> ResolveSectionAsync(CampaignRef campaignId, StreetRef streetId, HouseNumber houseNumber, CancellationToken cancellationToken)
    {
        var campaign = await _campaignQueries.GetByIdAsync(CampaignId.From(campaignId.Value), cancellationToken);

        if (campaign is not null && campaign.TerritoryId is not null)
        {
            var territoryStreetId = StreetId.From(streetId.Value);
            var territoryHouseNumber = TerritoryHouseNumber.Parse(houseNumber.ToString());

            var sections = await _streetSectionQueries.GetByTerritoryAndStreetAsync(TerritoryId.From(campaign.TerritoryId.Value), territoryStreetId, cancellationToken);

            var matchingSection = sections.FirstOrDefault(s => s.ContainsHouseNumber(territoryHouseNumber));

            if (matchingSection is not null)
            {
                return new SuccessfulSectionResolutionResult(StreetSectionRef.From(matchingSection.Id.Value), NeighborhoodRef.From(matchingSection.NeighborhoodId.Value));
            }

            return new OutOfBoundsSectionResolutionResult();
        }

        return null;
    }
}