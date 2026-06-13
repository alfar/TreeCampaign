using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public interface ISectionResolutionService
{
    Task<SectionResolutionResultBase?> ResolveSectionAsync(CampaignRef campaignId, StreetRef streetId, HouseNumber houseNumber, CancellationToken cancellationToken);
}