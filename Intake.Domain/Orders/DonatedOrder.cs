namespace Intake.Domain.Orders;

public class DonatedOrder : OrderBase
{
    private DonatedOrder() { }

    public static DonatedOrder CreateFrom(UnwashableOrder unwashableOrder)
    {
        var order = new DonatedOrder
        {
            Id = unwashableOrder.Id,
            CampaignId = unwashableOrder.CampaignId,
            Sender = unwashableOrder.Sender,
            Amount = unwashableOrder.Amount,
            OrderDate = unwashableOrder.OrderDate,
            Message = unwashableOrder.Message,
            TransactionId = unwashableOrder.TransactionId,
        };

        order.Raise(new Events.OrderMarkedAsDonation(order.Id, order.CampaignId));

        return order;
    }
}
