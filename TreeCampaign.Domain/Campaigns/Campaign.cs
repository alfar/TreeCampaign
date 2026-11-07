using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Domain.Campaigns;

public sealed class Campaign
{
    public required CampaignId Id { get; init; }
    public CampaignSeason Season { get; private set; }
    public TerritoryRef? TerritoryId { get; private set; }
    public required ScoutGroupRef ScoutGroupId { get; init; }
    public DateOnly? CollectionDate { get; private set; }
    public MoneyAmount? PricePerTree { get; private set; }
    public string? Title { get; private set; }
    public DateTimeOffset? LastOrderDateTime { get; private set; }

    /// <summary>Price used for campaigns that predate <see cref="PricePerTree"/> or haven't set one.</summary>
    public static readonly MoneyAmount DefaultPricePerTree = MoneyAmount.From(40m);

    public MoneyAmount EffectivePricePerTree => PricePerTree ?? DefaultPricePerTree;

    private Campaign() { Season = default!; }

    public static Campaign Create(
        CampaignSeason season,
        ScoutGroupRef scoutGroupId,
        TerritoryRef? territoryId = null,
        DateOnly? collectionDate = null,
        MoneyAmount? pricePerTree = null,
        string? title = null,
        DateTimeOffset? lastOrderDateTime = null)
    {
        return new Campaign
        {
            Id = CampaignId.From(Guid.NewGuid()),
            Season = season,
            TerritoryId = territoryId,
            ScoutGroupId = scoutGroupId,
            CollectionDate = collectionDate,
            PricePerTree = pricePerTree,
            Title = NormalizeTitle(title),
            LastOrderDateTime = lastOrderDateTime,
        };
    }

    public void SetSeason(CampaignSeason season)
    {
        Season = season;
    }

    public void SetTerritory(TerritoryRef territoryId)
    {
        TerritoryId = territoryId;
    }

    public void SetCollectionDate(DateOnly? collectionDate)
    {
        CollectionDate = collectionDate;
    }

    public void SetPricePerTree(MoneyAmount? pricePerTree)
    {
        PricePerTree = pricePerTree;
    }

    public void SetTitle(string? title)
    {
        Title = NormalizeTitle(title);
    }

    public void SetLastOrderDateTime(DateTimeOffset? lastOrderDateTime)
    {
        LastOrderDateTime = lastOrderDateTime;
    }

    public int CalculateTreeCount(decimal amount) => (int)Math.Floor(amount / EffectivePricePerTree.Value);

    private static string? NormalizeTitle(string? title) => string.IsNullOrWhiteSpace(title) ? null : title.Trim();
}
