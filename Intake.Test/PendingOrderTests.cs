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

    private static PendingOrder NewPending() => PendingOrder.Create(
        CampaignRef.From(Guid.NewGuid()),
        new Sender("Carol", "555"),
        MoneyAmount.From(80m),
        StreetRef.From(Guid.NewGuid()),
        StreetSectionRef.From(Guid.NewGuid()),
        NeighborhoodRef.From(Guid.NewGuid()),
        HouseNumber.Parse("3"),
        "carol@example.com");

    [Fact]
    public void Delete_ReturnsDeletedOrder_KeepingAllData_AndRaisesOrderDeleted()
    {
        var pending = NewPending();

        var deleted = pending.Delete();

        Assert.Equal(pending.Id, deleted.Id);
        Assert.Equal(pending.CampaignId, deleted.CampaignId);
        Assert.Equal(pending.Sender, deleted.Sender);
        Assert.Equal(pending.Email, deleted.Email);
        Assert.Equal(pending.Amount, deleted.Amount);
        Assert.Equal(pending.OrderDate, deleted.OrderDate);
        Assert.Equal(pending.Message, deleted.Message);
        Assert.Equal(pending.StreetId, deleted.StreetId);
        Assert.Equal(pending.StreetSectionId, deleted.StreetSectionId);
        Assert.Equal(pending.NeighborhoodId, deleted.NeighborhoodId);
        Assert.Equal(pending.HouseNumber, deleted.HouseNumber);

        var raised = Assert.IsType<Intake.Domain.Orders.Events.OrderDeleted>(Assert.Single(deleted.NewEvents));
        Assert.Equal(pending.Id, raised.Id);
        Assert.Equal(pending.CampaignId, raised.CampaignId);
    }

    [Fact]
    public void Undelete_RestoresEquivalentPendingOrder_AndRaisesOrderUndeleted()
    {
        var pending = NewPending();

        var restored = pending.Delete().Undelete();

        Assert.Equal(pending.Id, restored.Id);
        Assert.Equal(pending.CampaignId, restored.CampaignId);
        Assert.Equal(pending.Sender, restored.Sender);
        Assert.Equal(pending.Email, restored.Email);
        Assert.Equal(pending.Amount, restored.Amount);
        Assert.Equal(pending.OrderDate, restored.OrderDate);
        Assert.Equal(pending.Message, restored.Message);
        Assert.Equal(pending.StreetId, restored.StreetId);
        Assert.Equal(pending.StreetSectionId, restored.StreetSectionId);
        Assert.Equal(pending.NeighborhoodId, restored.NeighborhoodId);
        Assert.Equal(pending.HouseNumber, restored.HouseNumber);

        var raised = Assert.IsType<Intake.Domain.Orders.Events.OrderUndeleted>(Assert.Single(restored.NewEvents));
        Assert.Equal(pending.Id, raised.Id);
    }
}
