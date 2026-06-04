namespace Intake.Domain.Orders.ValueObjects;

public sealed record OrderId(Guid Value)
{
    public static bool TryParse(string? input, out OrderId orderId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            orderId = From(guid);
            return true;
        }

        orderId = From(Guid.Empty);
        return false;
    }

    public static OrderId From(Guid value) => new OrderId(value);
}