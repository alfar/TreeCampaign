using System.Numerics;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public class PendingOrder : OrderBase
{
    public required HouseNumber HouseNumber { get; init; }
    public required StreetRef StreetId { get; init; }
    public required StreetSectionRef StreetSectionId { get; init; }
    public required NeighborhoodRef NeighborhoodId { get; init; }

    public static PendingOrder Create(
        CampaignRef campaignId,
        Sender sender,
        MoneyAmount amount,
        StreetRef streetId,
        StreetSectionRef streetSectionId,
        NeighborhoodRef neighborhoodId,
        HouseNumber houseNumber)
    {
        var orderId = OrderId.From(Guid.NewGuid());
        var paymentCode = GeneratePaymentCode(orderId);

        var order = new PendingOrder
        {
            Id = orderId,
            CampaignId = campaignId,
            Sender = sender,
            Amount = amount,
            OrderDate = DateTimeOffset.UtcNow,
            Message = paymentCode,
            StreetId = streetId,
            StreetSectionId = streetSectionId,
            NeighborhoodId = neighborhoodId,
            HouseNumber = houseNumber
        };

        order.Raise(new Events.OrderPending(order.Id, order.CampaignId));

        return order;
    }

    internal static string GeneratePaymentCode(OrderId orderId)
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var value = new BigInteger(orderId.Value.ToByteArray(), isUnsigned: true, isBigEndian: true);

        if (value < 0)
        {
            value = BigInteger.Abs(value);
        }

        var chars = new char[4];
        for (var i = 0; i < chars.Length; i++)
        {
            var index = (int)(value % alphabet.Length);
            chars[i] = alphabet[index];
            value /= alphabet.Length;
        }

        return new string(chars.Reverse().ToArray());
    }

    public WashedOrder ConfirmPaid(TransactionId? transactionId = null)
    {
        return WashedOrder.CreateFrom(this, transactionId ?? TransactionId);
    }

    private PendingOrder() { }
}

