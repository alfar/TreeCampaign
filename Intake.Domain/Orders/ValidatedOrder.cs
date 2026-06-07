using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class ValidatedOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required StreetSectionRef StreetSectionId { get; init; }
    public required NeighborhoodRef NeighborhoodId { get; init; }

    private ValidatedOrder() { }

    public static ValidatedOrder CreateFrom(
        IncomingOrder incomingOrder,
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber
    )
    {
        var order = new ValidatedOrder
        {
            Id = incomingOrder.Id,
            CampaignId = incomingOrder.CampaignId,
            Sender = incomingOrder.Sender,
            Amount = incomingOrder.Amount,
            OrderDate = incomingOrder.OrderDate,
            Message = incomingOrder.Message,
            StreetId = streetId,
            StreetSectionId = streetSectionId,
            NeighborhoodId = neighborhoodId,
            HouseNumber = houseNumber
        };
    
        order.Raise(new Events.OrderValidated(order.Id));

        return order;
    }

    public static ValidatedOrder CreateFrom(
        UnwashedOrder unwashedOrder,
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber
    )
    {
        var order = new ValidatedOrder
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

        order.Raise(new Events.OrderValidated(order.Id));

        return order;
    }

    public static ValidatedOrder CreateFrom(
        WashedOrder washedOrder,
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber
    )
    {
        var order = new ValidatedOrder
        {
            Id = washedOrder.Id,
            CampaignId = washedOrder.CampaignId,
            Sender = washedOrder.Sender,
            Amount = washedOrder.Amount,
            OrderDate = washedOrder.OrderDate,
            Message = washedOrder.Message,
            StreetId = streetId,
            StreetSectionId = streetSectionId,
            NeighborhoodId = neighborhoodId,
            HouseNumber = houseNumber
        };

        order.Raise(new Events.OrderValidated(order.Id));

        return order;
    }

    public static ValidatedOrder CreateFrom(
        OutOfBoundsOrder outOfBoundsOrder,
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber
    )
    {
        var order = new ValidatedOrder
        {
            Id = outOfBoundsOrder.Id,
            CampaignId = outOfBoundsOrder.CampaignId,
            Sender = outOfBoundsOrder.Sender,
            Amount = outOfBoundsOrder.Amount,
            OrderDate = outOfBoundsOrder.OrderDate,
            Message = outOfBoundsOrder.Message,
            StreetId = streetId,
            StreetSectionId = streetSectionId,
            NeighborhoodId = neighborhoodId,
            HouseNumber = houseNumber
        };

        order.Raise(new Events.OrderValidated(order.Id));

        return order;
    }
}