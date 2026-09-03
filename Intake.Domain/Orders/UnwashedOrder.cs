using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class UnwashedOrder : OrderBase, IParseableOrder
{
    public string? ErrorMessage { get; private set; }

    public static UnwashedOrder CreateFrom(IncomingOrder incomingOrder, string? errorMessage = null)
    {
        var order = new UnwashedOrder
        {
            Id = incomingOrder.Id,
            CampaignId = incomingOrder.CampaignId,
            Sender = incomingOrder.Sender,
            Amount = incomingOrder.Amount,
            OrderDate = incomingOrder.OrderDate,
            Message = incomingOrder.Message,
            TransactionId = incomingOrder.TransactionId,
            ErrorMessage = errorMessage,
        };

        order.Raise(new Events.OrderMarkedUnwashed(incomingOrder.Id, incomingOrder.CampaignId, errorMessage));

        return order;
    }

    public static UnwashedOrder CreateFrom(WashedOrder washedOrder, string errorMessage)
    {
        var order = new UnwashedOrder
        {
            Id = washedOrder.Id,
            CampaignId = washedOrder.CampaignId,
            Sender = washedOrder.Sender,
            Amount = washedOrder.Amount,
            OrderDate = washedOrder.OrderDate,
            Message = washedOrder.Message,
            TransactionId = washedOrder.TransactionId,
            ErrorMessage = errorMessage,
        };

        order.Raise(new Events.OrderMarkedUnwashed(washedOrder.Id, washedOrder.CampaignId, errorMessage));

        return order;
    }

    public static UnwashedOrder CreateFrom(OutOfBoundsOrder outOfBoundsOrder, string errorMessage)
    {
        var order = new UnwashedOrder
        {
            Id = outOfBoundsOrder.Id,
            CampaignId = outOfBoundsOrder.CampaignId,
            Sender = outOfBoundsOrder.Sender,
            Amount = outOfBoundsOrder.Amount,
            OrderDate = outOfBoundsOrder.OrderDate,
            Message = outOfBoundsOrder.Message,
            TransactionId = outOfBoundsOrder.TransactionId,
            ErrorMessage = errorMessage,
        };

        order.Raise(new Events.OrderMarkedUnwashed(outOfBoundsOrder.Id, outOfBoundsOrder.CampaignId, errorMessage));

        return order;
    }

    public static UnwashedOrder CreateFrom(UnwashableOrder unwashableOrder)
    {
        var order = new UnwashedOrder
        {
            Id = unwashableOrder.Id,
            CampaignId = unwashableOrder.CampaignId,
            Sender = unwashableOrder.Sender,
            Amount = unwashableOrder.Amount,
            OrderDate = unwashableOrder.OrderDate,
            Message = unwashableOrder.Message,
            TransactionId = unwashableOrder.TransactionId,
        };

        return order;
    }

    public WashedOrder Wash(StreetRef streetId, StreetSectionRef streetSectionId, NeighborhoodRef neighborhoodId, HouseNumber houseNumber)
    {
        return WashedOrder.CreateFrom(this, streetId, streetSectionId, neighborhoodId, houseNumber);
    }

    public OutOfBoundsOrder MarkOutOfBounds(StreetRef streetId, HouseNumber houseNumber)
    {
        return OutOfBoundsOrder.CreateFrom(this, streetId, houseNumber);
    }

    public UnwashableOrder MarkUnwashable()
    {
        return UnwashableOrder.CreateFrom(this);
    }

    public ValidatedOrder Accept(ValidationSuccess result)
    {
        return ValidatedOrder.CreateFrom(this, result.StreetId, result.StreetSectionId, result.NeighborhoodId, result.HouseNumber, result.Latitude, result.Longitude);
    }

    public void UpdateErrorMessage(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    private UnwashedOrder() { }
}

