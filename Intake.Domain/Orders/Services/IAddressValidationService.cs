using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public interface IAddressValidationService
{
    Task<AddressValidationResult> ValidateAsync(
        ParsedAddress address,
        CampaignRef campaignId,
        CancellationToken cancellationToken = default);
    Task<AddressValidationResult> ValidateRefsAsync(
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber,
        CampaignRef campaignId,
        CancellationToken cancellationToken = default);
    Task<AddressValidationResult> ValidateStreetAsync(
        StreetRef streetId,
        HouseNumber houseNumber,
        CampaignRef campaignId,
        CancellationToken cancellationToken = default);
    
    Task<bool> DoesTerritoryExistAsync(TerritoryRef territoryId, CancellationToken cancellationToken);
}
