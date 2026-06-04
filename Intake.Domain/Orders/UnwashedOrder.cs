using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class UnwashedOrder : OrderBase
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

    public WashedOrder Wash(StreetRef streetId, StreetSectionRef streetSectionId, NeighborhoodRef neighborhoodId)
    {
        return WashedOrder.CreateFrom(this, streetId, streetSectionId, neighborhoodId);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId);
    }

    private UnwashedOrder() { }
}

