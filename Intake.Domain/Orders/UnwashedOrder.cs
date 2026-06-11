using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class UnwashedOrder : OrderBase, IParseableOrder
{
    public static UnwashedOrder CreateFrom(IncomingOrder incomingOrder)
    {
        var order = new UnwashedOrder
        {
            Id = incomingOrder.Id,
            CampaignId = incomingOrder.CampaignId,
            Sender = incomingOrder.Sender,
            Amount = incomingOrder.Amount,
            OrderDate = incomingOrder.OrderDate,
            Message = incomingOrder.Message,
        };

        order.Raise(new Events.OrderMarkedUnwashed(incomingOrder.Id));

        return order;
    }

    public WashedOrder Wash(StreetRef streetId, StreetSectionRef streetSectionId, NeighborhoodRef neighborhoodId, HouseNumber houseNumber)
    {
        return WashedOrder.CreateFrom(this, streetId, streetSectionId, neighborhoodId, houseNumber);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId, result.HouseNumber, result.Latitude, result.Longitude);
    }

    private UnwashedOrder() { }
}

