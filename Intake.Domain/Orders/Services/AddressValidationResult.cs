using Intake.Domain.ExternalReferences;

namespace Intake.Domain.Orders.Services;

public abstract record AddressValidationResult;

public sealed record ValidationSuccess(
    TerritoryRef TerritoryId,
    NeighborhoodRef NeighborhoodId,
    StreetRef StreetId,
    StreetSectionRef StreetSectionId) : AddressValidationResult;

public sealed record StreetNotFound() : AddressValidationResult;

public sealed record HouseNumberOutOfBounds(StreetRef StreetId) : AddressValidationResult;
