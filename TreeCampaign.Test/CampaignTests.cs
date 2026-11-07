using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Test;

public class CampaignTests
{
    private static Campaign CreateCampaign() =>
        Campaign.Create(CampaignSeason.From(2026), ScoutGroupRef.From(Guid.NewGuid()));

    [Fact]
    public void Create_WithoutOptionalDetails_LeavesThemNull()
    {
        var campaign = CreateCampaign();

        Assert.Null(campaign.CollectionDate);
        Assert.Null(campaign.PricePerTree);
        Assert.Null(campaign.Title);
        Assert.Null(campaign.LastOrderDateTime);
    }

    [Fact]
    public void Create_WithDetails_StoresThem()
    {
        var date = new DateOnly(2027, 1, 10);
        var deadline = new DateTimeOffset(2027, 1, 5, 20, 0, 0, TimeSpan.FromHours(1));

        var campaign = Campaign.Create(
            CampaignSeason.From(2026),
            ScoutGroupRef.From(Guid.NewGuid()),
            collectionDate: date,
            pricePerTree: MoneyAmount.From(40m),
            title: "  Juletræer 2026 ",
            lastOrderDateTime: deadline);

        Assert.Equal(date, campaign.CollectionDate);
        Assert.Equal(MoneyAmount.From(40m), campaign.PricePerTree);
        Assert.Equal("Juletræer 2026", campaign.Title);
        Assert.Equal(deadline, campaign.LastOrderDateTime);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetTitle_BlankValue_ClearsTitle(string? title)
    {
        var campaign = CreateCampaign();
        campaign.SetTitle("Something");

        campaign.SetTitle(title);

        Assert.Null(campaign.Title);
    }

    [Theory]
    [InlineData(null, 80, 2)]
    [InlineData(null, 79, 1)]
    [InlineData(50, 100, 2)]
    [InlineData(50, 99, 1)]
    public void CalculateTreeCount_UsesPricePerTree_AndDefaultsTo40(int? price, decimal amount, int expected)
    {
        var campaign = CreateCampaign();
        campaign.SetPricePerTree(price is null ? null : MoneyAmount.From(price.Value));

        Assert.Equal(expected, campaign.CalculateTreeCount(amount));
    }
}
