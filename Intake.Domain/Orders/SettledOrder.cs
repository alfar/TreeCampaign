using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class SettledOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required TerritoryRef TerritoryId { get; init; }

    private SettledOrder() { }

    public static SettledOrder CreateFrom(TransferredOrder transferredOrder)
    {
        var order = new SettledOrder
        {
            Id = transferredOrder.Id,
            CampaignId = transferredOrder.CampaignId,
            Sender = transferredOrder.Sender,
            Amount = transferredOrder.Amount,
            OrderDate = transferredOrder.OrderDate,
            Message = transferredOrder.Message,
            TransactionId = transferredOrder.TransactionId,
            StreetId = transferredOrder.StreetId,
            HouseNumber = transferredOrder.HouseNumber,
            TerritoryId = transferredOrder.TerritoryId
        };

        order.Raise(new Events.OrderSettled(order.Id, order.CampaignId, order.TerritoryId));

        return order;
    }
}
