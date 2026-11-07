namespace TreeCampaign.Domain.Campaigns.ValueObjects;

public record MoneyAmount(decimal Value)
{
    public static MoneyAmount From(decimal value) => value > 0 ? new(value) : throw new ArgumentOutOfRangeException(nameof(value));
}
