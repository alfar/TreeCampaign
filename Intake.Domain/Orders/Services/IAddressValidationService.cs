using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public interface IAddressValidationService
{
    Task<AddressValidationResult> ValidateAsync(
        ParsedAddress address,
        CampaignRef campaignId,
        CancellationToken cancellationToken = default);
}
