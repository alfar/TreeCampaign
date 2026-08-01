using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class TransferredOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required TerritoryRef TerritoryId { get; init; }

    private TransferredOrder() { }

    public static TransferredOrder CreateFrom(
        OutOfBoundsOrder outOfBoundsOrder,
        TerritoryRef territoryId
    )
    {
        var order = new TransferredOrder
        {
            Id = outOfBoundsOrder.Id,
            CampaignId = outOfBoundsOrder.CampaignId,
            Sender = outOfBoundsOrder.Sender,
            Amount = outOfBoundsOrder.Amount,
            OrderDate = outOfBoundsOrder.OrderDate,
            Message = outOfBoundsOrder.Message,
            TransactionId = outOfBoundsOrder.TransactionId,
            StreetId = outOfBoundsOrder.StreetId,
            HouseNumber = outOfBoundsOrder.HouseNumber,
            TerritoryId = territoryId
        };

        order.Raise(new Events.OrderTransferred(order.Id, order.CampaignId, order.TerritoryId));

        return order;
    }

    public OutOfBoundsOrder UndoTransfer()
    {
        return OutOfBoundsOrder.CreateFrom(this);
    }

    public SettledOrder MarkAsPaid()
    {
        return SettledOrder.CreateFrom(this);
    }
}