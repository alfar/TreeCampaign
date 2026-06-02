using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class WashedOrder : OrderBase
{
    public required WashedAddress WashedAddress { get; init; }

    public static WashedOrder CreateFrom(UnwashedOrder unwashedOrder, WashedAddress washedAddress)
    {
        var order = new WashedOrder
        {
            Id = unwashedOrder.Id,
            CampaignId = unwashedOrder.CampaignId,
            Sender = unwashedOrder.Sender,
            Amount = unwashedOrder.Amount,
            OrderDate = unwashedOrder.OrderDate,
            Message = unwashedOrder.Message,
            WashedAddress = washedAddress
        };

        order.Raise(new Events.OrderWashed(unwashedOrder.Id));

        return order;
    }

    public OutOfBoundsOrder MarkOutOfBounds(HouseNumberOutOfBounds result)
    {
        return OutOfBoundsOrder.CreateFrom(this, result.StreetId);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId);
    }

    private WashedOrder() { }
}

