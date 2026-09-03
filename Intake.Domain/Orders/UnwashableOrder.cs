namespace Intake.Domain.Orders;

public class UnwashableOrder : OrderBase
{
    private UnwashableOrder() { }

    public static UnwashableOrder CreateFrom(UnwashedOrder unwashedOrder)
    {
        var order = new UnwashableOrder
        {
            Id = unwashedOrder.Id,
            CampaignId = unwashedOrder.CampaignId,
            Sender = unwashedOrder.Sender,
            Amount = unwashedOrder.Amount,
            OrderDate = unwashedOrder.OrderDate,
            Message = unwashedOrder.Message,
            TransactionId = unwashedOrder.TransactionId,
        };

        order.Raise(new Events.OrderMarkedUnwashable(order.Id, order.CampaignId));

        return order;
    }

    public static UnwashableOrder CreateFrom(OutOfBoundsOrder outOfBoundsOrder)
    {
        var order = new UnwashableOrder
        {
            Id = outOfBoundsOrder.Id,
            CampaignId = outOfBoundsOrder.CampaignId,
            Sender = outOfBoundsOrder.Sender,
            Amount = outOfBoundsOrder.Amount,
            OrderDate = outOfBoundsOrder.OrderDate,
            Message = outOfBoundsOrder.Message,
            TransactionId = outOfBoundsOrder.TransactionId,
        };

        order.Raise(new Events.OrderMarkedUnwashable(order.Id, order.CampaignId));

        return order;
    }

    public UnwashedOrder UndoMarkUnwashable()
    {
        var order = UnwashedOrder.CreateFrom(this);

        Raise(new Events.OrderUnwashableUndone(Id, CampaignId));

        return order;
    }

    public RefundedOrder Refund()
    {
        return RefundedOrder.CreateFrom(this);
    }

    public DonatedOrder MarkAsDonation()
    {
        return DonatedOrder.CreateFrom(this);
    }
}
