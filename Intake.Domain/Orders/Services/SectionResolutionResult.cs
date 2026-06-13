using Intake.Domain.ExternalReferences;

namespace Intake.Domain.Orders.Services;

public abstract record SectionResolutionResultBase();

public record SuccessfulSectionResolutionResult(StreetSectionRef StreetSectionId, NeighborhoodRef NeighborhoodId) : SectionResolutionResultBase;

public record OutOfBoundsSectionResolutionResult() : SectionResolutionResultBase;
