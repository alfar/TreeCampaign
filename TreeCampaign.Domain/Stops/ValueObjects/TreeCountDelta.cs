namespace TreeCampaign.Domain.Stops.ValueObjects;

public sealed record TreeCountDelta(int Value)
{
    public static TreeCountDelta From(int value) => new(value);
}
