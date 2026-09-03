namespace Intake.Domain.Orders;

public class RefundedOrder : OrderBase
{
    private RefundedOrder() { }

    public static RefundedOrder CreateFrom(UnwashableOrder unwashableOrder)
    {
        var order = new RefundedOrder
        {
            Id = unwashableOrder.Id,
            CampaignId = unwashableOrder.CampaignId,
            Sender = unwashableOrder.Sender,
            Amount = unwashableOrder.Amount,
            OrderDate = unwashableOrder.OrderDate,
            Message = unwashableOrder.Message,
            TransactionId = unwashableOrder.TransactionId,
        };

        order.Raise(new Events.OrderRefunded(order.Id, order.CampaignId));

        return order;
    }
}
