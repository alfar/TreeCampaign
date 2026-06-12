using Intake.Domain.ExternalReferences;

namespace Intake.Domain.Orders.Services;

public record SectionResolutionResult(StreetSectionRef StreetSectionId, NeighborhoodRef NeighborhoodId);
