using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class PendingOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required StreetSectionRef StreetSectionId { get; init; }
    public required NeighborhoodRef NeighborhoodId { get; init; }

    public static PendingOrder Create(
        CampaignRef campaignId,
        Sender sender,
        MoneyAmount amount,
        DateTimeOffset orderDate,
        string message,
        TransactionId? transactionId,
         StreetRef streetId, StreetSectionRef streetSectionId, NeighborhoodRef neighborhoodId, HouseNumber houseNumber)
    {
        var order = new PendingOrder
        {
            Id = OrderId.From(Guid.NewGuid()),
            CampaignId = campaignId,
            Sender = sender,
            Amount = amount,
            OrderDate = orderDate,
            Message = message,
            TransactionId = transactionId,
            StreetId = streetId,
            StreetSectionId = streetSectionId,
            NeighborhoodId = neighborhoodId,
            HouseNumber = houseNumber
        };

        order.Raise(new Events.OrderPending(order.Id, order.CampaignId));

        return order;
    }

    public WashedOrder ConfirmPaid()
    {
        return WashedOrder.CreateFrom(this);
    }

    private PendingOrder() { }
}

