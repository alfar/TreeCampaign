namespace Intake.Domain.Orders.ValueObjects;

public sealed record TransactionId(string Value)
{
    public static TransactionId From(string value) => new(value);
}
