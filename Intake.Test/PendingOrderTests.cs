using System;
using Xunit;
using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;
using Intake.Domain.ExternalReferences;

namespace Intake.Test;

public class PendingOrderTests
{
    [Fact]
    public void Create_GeneratesPaymentCode_SetsProperties()
    {
        var campaign = CampaignRef.From(Guid.NewGuid());
        var sender = new Sender("Alice", "12345");
        var amount = MoneyAmount.From(40m);
        var street = StreetRef.From(Guid.NewGuid());
        var section = StreetSectionRef.From(Guid.NewGuid());
        var neighborhood = NeighborhoodRef.From(Guid.NewGuid());
        var house = new HouseNumber(12);

        var order = PendingOrder.Create(campaign, sender, amount, street, section, neighborhood, house);

        Assert.NotNull(order);
        Assert.NotEqual(Guid.Empty, order.Id.Value);
        Assert.Equal(amount, order.Amount);
        Assert.Equal(street, order.StreetId);
        Assert.Equal(section, order.StreetSectionId);
        Assert.Equal(neighborhood, order.NeighborhoodId);
        Assert.Equal(house, order.HouseNumber);
        Assert.False(string.IsNullOrWhiteSpace(order.Message));
        Assert.Equal(4, order.Message.Length);

        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        foreach (var c in order.Message)
        {
            Assert.Contains(c, alphabet);
        }

        Assert.True((DateTimeOffset.UtcNow - order.OrderDate).TotalSeconds < 60);
    }

    [Fact]
    public void ConfirmPaid_Returns_WashedOrder_WithSameIdsAndMessage()
    {
        var campaign = CampaignRef.From(Guid.NewGuid());
        var sender = new Sender("Bob", null);
        var amount = MoneyAmount.From(40m);
        var street = StreetRef.From(Guid.NewGuid());
        var section = StreetSectionRef.From(Guid.NewGuid());
        var neighborhood = NeighborhoodRef.From(Guid.NewGuid());
        var house = HouseNumber.Parse("7");

        var pending = PendingOrder.Create(campaign, sender, amount, street, section, neighborhood, house);
        var washed = pending.ConfirmPaid();

        Assert.IsType<WashedOrder>(washed);
        Assert.Equal(pending.Id, washed.Id);
        Assert.Equal(pending.CampaignId, washed.CampaignId);
        Assert.Equal(pending.Message, washed.Message);
        Assert.Equal(pending.StreetId, washed.StreetId);
        Assert.Equal(pending.HouseNumber, washed.HouseNumber);
    }
}
