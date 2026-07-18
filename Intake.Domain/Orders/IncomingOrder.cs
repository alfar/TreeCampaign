using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class IncomingOrder : OrderBase, IParseableOrder
{
    public static IncomingOrder Create(
        CampaignRef campaignId,
        Sender sender,
        MoneyAmount amount,
        DateTimeOffset orderDate,
        string message,
        TransactionId? transactionId = null
    )
    {
        var order = new IncomingOrder
        {
            Id = OrderId.From(Guid.NewGuid()),
            CampaignId = campaignId,
            Sender = sender,
            Amount = amount,
            OrderDate = orderDate,
            Message = message,
            TransactionId = transactionId
        };

        order.Raise(new Events.OrderReceived(order.Id, campaignId, sender, amount, orderDate, message));

        return order;
    }

    public UnwashedOrder MarkUnwashed()
    {
        return UnwashedOrder.CreateFrom(this);
    }

    public UnwashedOrder MarkUnwashed(string errorMessage)
    {
        return UnwashedOrder.CreateFrom(this, errorMessage);
    }

    public OutOfBoundsOrder MarkOutOfBounds(HouseNumberOutOfBounds result)
    {
        return OutOfBoundsOrder.CreateFrom(this, result.StreetId, result.HouseNumber);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId, result.HouseNumber, result.Latitude, result.Longitude);
    }

    private IncomingOrder() { }    
}