using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;

namespace Intake.Domain.Orders;

public class WashedOrder : OrderBase
{
    public required StreetRef StreetId { get; init; }
    public required StreetSectionRef StreetSectionId { get; init; }
    public required NeighborhoodRef NeighborhoodId { get; init; }

    public static WashedOrder CreateFrom(UnwashedOrder unwashedOrder, StreetRef streetId, StreetSectionRef streetSectionId, NeighborhoodRef neighborhoodId)
    {
        var order = new WashedOrder
        {
            Id = unwashedOrder.Id,
            CampaignId = unwashedOrder.CampaignId,
            Sender = unwashedOrder.Sender,
            Amount = unwashedOrder.Amount,
            OrderDate = unwashedOrder.OrderDate,
            Message = unwashedOrder.Message,
            StreetId = streetId,
            StreetSectionId = streetSectionId,
            NeighborhoodId = neighborhoodId
        };

        order.Raise(new Events.OrderWashed(unwashedOrder.Id));

        return order;
    }

    public OutOfBoundsOrder MarkOutOfBounds()
    {
        return OutOfBoundsOrder.CreateFrom(this, StreetId);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId);
    }

    private WashedOrder() { }
}

