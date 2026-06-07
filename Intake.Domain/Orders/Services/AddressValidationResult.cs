using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public abstract record AddressValidationResult;

public sealed record ValidationSuccess(
    TerritoryRef TerritoryId,
    NeighborhoodRef NeighborhoodId,
    StreetRef StreetId,
    StreetSectionRef StreetSectionId,
    HouseNumber HouseNumber) : AddressValidationResult;

public sealed record StreetNotFound() : AddressValidationResult;

public sealed record HouseNumberOutOfBounds(StreetRef StreetId) : AddressValidationResult;
