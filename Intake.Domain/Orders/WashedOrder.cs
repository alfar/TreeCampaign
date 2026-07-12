using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class WashedOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required StreetSectionRef StreetSectionId { get; init; }
    public required NeighborhoodRef NeighborhoodId { get; init; }

    public static WashedOrder CreateFrom(UnwashedOrder unwashedOrder, StreetRef streetId, StreetSectionRef streetSectionId, NeighborhoodRef neighborhoodId, HouseNumber houseNumber)
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
            NeighborhoodId = neighborhoodId,
            HouseNumber = houseNumber
        };

        order.Raise(new Events.OrderWashed(unwashedOrder.Id, unwashedOrder.CampaignId));

        return order;
    }

    public OutOfBoundsOrder MarkOutOfBounds()
    {
        return OutOfBoundsOrder.CreateFrom(this, StreetId);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId, result.HouseNumber, result.Latitude, result.Longitude);
    }

    private WashedOrder() { }
}

