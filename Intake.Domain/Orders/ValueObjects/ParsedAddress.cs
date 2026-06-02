namespace Intake.Domain.Orders.ValueObjects;

public record ParsedAddress(string Street, string HouseNumber, string? ZipCode, string? City);
