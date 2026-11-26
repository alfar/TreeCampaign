using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

/// <summary>
/// A pending order the operator has thrown in the trash ("Papirkurv"). It keeps the address
/// fields of the PendingOrder so <see cref="Undelete"/> can restore it exactly as it was.
/// </summary>
public class DeletedOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required StreetSectionRef StreetSectionId { get; init; }
    public required NeighborhoodRef NeighborhoodId { get; init; }

    private DeletedOrder() { }

    public static DeletedOrder CreateFrom(PendingOrder pendingOrder)
    {
        var order = new DeletedOrder
        {
            Id = pendingOrder.Id,
            CampaignId = pendingOrder.CampaignId,
            Sender = pendingOrder.Sender,
            Email = pendingOrder.Email,
            Amount = pendingOrder.Amount,
            OrderDate = pendingOrder.OrderDate,
            Message = pendingOrder.Message,
            TransactionId = pendingOrder.TransactionId,
            StreetId = pendingOrder.StreetId,
            StreetSectionId = pendingOrder.StreetSectionId,
            NeighborhoodId = pendingOrder.NeighborhoodId,
            HouseNumber = pendingOrder.HouseNumber
        };

        order.Raise(new Events.OrderDeleted(order.Id, order.CampaignId));

        return order;
    }

    public PendingOrder Undelete()
    {
        return PendingOrder.CreateFrom(this);
    }
}
