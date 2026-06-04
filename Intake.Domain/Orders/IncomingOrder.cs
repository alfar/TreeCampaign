using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class IncomingOrder : OrderBase
{
    public static IncomingOrder Create(
        CampaignRef campaignId,
        Sender sender,
        MoneyAmount amount,
        DateTimeOffset orderDate,
        string message
    )
    {
        var order = new IncomingOrder
        {
            Id = OrderId.From(Guid.NewGuid()),
            CampaignId = campaignId,
            Sender = sender,
            Amount = amount,
            OrderDate = orderDate,
            Message = message
        };

        order.Raise(new Events.OrderReceived(order.Id));

        return order;
    }

    public UnwashedOrder MarkUnwashed()
    {
        return UnwashedOrder.CreateFrom(this);
    }

    public OutOfBoundsOrder MarkOutOfBounds(HouseNumberOutOfBounds result)
    {
        return OutOfBoundsOrder.CreateFrom(this, result.StreetId);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId);
    }

    private IncomingOrder() { }    
}