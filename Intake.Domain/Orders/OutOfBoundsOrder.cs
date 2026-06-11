using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;

namespace Intake.Domain.Orders;

public class OutOfBoundsOrder : OrderBase, IParseableOrder
{
    public required StreetRef StreetId { get; init; }

    public static OutOfBoundsOrder CreateFrom(IncomingOrder incomingOrder, StreetRef streetId)
    {
        var order = new OutOfBoundsOrder
        {
            Id = incomingOrder.Id,
            CampaignId = incomingOrder.CampaignId,
            Sender = incomingOrder.Sender,
            Amount = incomingOrder.Amount,
            OrderDate = incomingOrder.OrderDate,
            Message = incomingOrder.Message,
            StreetId = streetId
        };

        order.Raise(new Events.OrderMarkedOutOfBounds(incomingOrder.Id));

        return order;
    }

    public static OutOfBoundsOrder CreateFrom(WashedOrder washedOrder, StreetRef streetId)
    {
        var order = new OutOfBoundsOrder
        {
            Id = washedOrder.Id,
            CampaignId = washedOrder.CampaignId,
            Sender = washedOrder.Sender,
            Amount = washedOrder.Amount,
            OrderDate = washedOrder.OrderDate,
            Message = washedOrder.Message,
            StreetId = streetId
        };

        order.Raise(new Events.OrderMarkedOutOfBounds(washedOrder.Id));

        return order;
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId, result.HouseNumber, result.Latitude, result.Longitude);
    }

    private OutOfBoundsOrder() { }
}

