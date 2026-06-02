using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public interface IAddressParser
{
    ParsedAddress? TryParse(string message);
}
